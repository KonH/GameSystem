using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Client.Abstractions;
using Core.Client.Shared;
using Core.Common.Command;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;
using Core.Common.Utils;
using Core.Service.Model;
using Core.Service.Queue;
using Core.Service.UseCase.GetConfig;
using Core.Service.UseCase.GetState;
using Core.Service.UseCase.UpdateState;
using Core.Service.UseCase.WaitCommand;

namespace Core.Client.Embedded {
	public sealed class EmbeddedServiceWaitClient<TConfig, TState> : IClient<TConfig, TState>
		where TConfig : IConfig where TState : class, IState, new() {
		readonly UserId _userId;

		readonly ILogger<EmbeddedServiceWaitClient<TConfig, TState>> _logger;
		readonly CommandExecutor<TConfig, TState>                    _singleExecutor;

		readonly GetConfigUseCase<TConfig>           _getConfigUseCase;
		readonly GetStateUseCase<TState>             _getStateUseCase;
		readonly UpdateStateUseCase<TConfig, TState> _updateStateUseCase;
		readonly WaitCommandUseCase<TConfig, TState> _waitCommandUseCase;
		readonly CommandScheduler<TConfig, TState>   _scheduler;

		public TState  State  { get; private set; }
		public TConfig Config { get; private set; }

		public EmbeddedServiceWaitClient(
			ILoggerFactory                      loggerFactory,
			CommandExecutor<TConfig, TState>    commandExecutor,
			GetConfigUseCase<TConfig>           getConfigUseCase,
			GetStateUseCase<TState>             getStateUseCase,
			UpdateStateUseCase<TConfig, TState> updateStateUseCase,
			WaitCommandUseCase<TConfig, TState> waitCommandUseCase,
			UserIdSource                        userIdSource,
			CommandScheduler<TConfig, TState>   scheduler) {
			_userId             = userIdSource.GetOrCreateUserId();
			_logger             = loggerFactory.Create<EmbeddedServiceWaitClient<TConfig, TState>>();
			_getConfigUseCase   = getConfigUseCase;
			_getStateUseCase    = getStateUseCase;
			_updateStateUseCase = updateStateUseCase;
			_waitCommandUseCase = waitCommandUseCase;
			_singleExecutor     = commandExecutor;
			_scheduler          = scheduler;
		}

		public async Task<InitializationResult> Initialize(CancellationToken cancellationToken) {
			try {
				await UpdateConfig();
				await UpdateState();
				var _  = Task.Run(WaitCommand, cancellationToken);
				var __ = Task.Run(ProcessCommand, cancellationToken);
			} catch ( Exception e ) {
				return new InitializationResult.Error(e.ToString());
			}
			return new InitializationResult.Ok();
		}

		public async Task<CommandApplyResult> Apply(ICommand<TConfig, TState> command) {
			var request  = new UpdateStateRequest<TConfig, TState>(_userId, State.Version, Config.Version, command);
			var response = await _updateStateUseCase.Handle(request);
			switch ( response ) {
				case UpdateStateResponse.Updated<TConfig, TState> updated: {
					await _singleExecutor.Apply(Config, State, command, true);
					foreach ( var cmd in updated.NextCommands ) {
						await _singleExecutor.Apply(Config, State, cmd, true);
					}
					State.Version = updated.NewVersion;
					return new CommandApplyResult.Ok();
				}

				case object value: {
					return new CommandApplyResult.Error($"Result is '{value.GetType()}', force update state");
				}

				default: {
					return new CommandApplyResult.Error("Failed to apply command");
				}
			}
		}

		async Task WaitCommand() {
			while ( true ) {
				_logger.LogTrace("Awaiting for new command");
				var request = new WaitCommandRequest(_userId, State.Version, Config.Version);
				var response = await _waitCommandUseCase.Handle(request);
				switch ( response ) {
					case WaitCommandResponse.Updated<TConfig, TState> updated: {
						_logger.LogTrace($"New commands found: {updated.NextCommands.Count}");
						foreach ( var cmd in updated.NextCommands ) {
							await _singleExecutor.Apply(Config, State, cmd, true);
						}
						State.Version = updated.NewVersion;
						break;
					}

					case WaitCommandResponse.NotFound _: {
						_logger.LogTrace("No new commands found");
						break;
					}

					case object value: {
						_logger.LogError($"New command result is '{value.GetType()}'");
						break;
					}

					default: {
						_logger.LogError("Failed to get new command");
						break;
					}
				}
			}
			// ReSharper disable once FunctionNeverReturns
		}

		async Task ProcessCommand() {
			while ( true ) {
				_scheduler.Update();
				await Task.Delay(1);
			}
			// ReSharper disable once FunctionNeverReturns
		}

		async Task UpdateConfig() {
			_logger.LogTrace($"Update config for '{_userId}'");
			var request  = new GetConfigRequest(_userId);
			var response = await _getConfigUseCase.Handle(request);
			switch ( response ) {
				case GetConfigResponse.Found<TConfig> found: {
					Config = found.Config;
					_logger.LogTrace("Config found");
					break;
				}

				case object value: {
					_logger.LogError($"Result is '{value.GetType()}'");
					throw new InvalidOperationException();
				}
			}
		}

		async Task UpdateState() {
			_logger.LogTrace($"Update state for '{_userId}'");
			var request  = new GetStateRequest(_userId);
			var response = await _getStateUseCase.Handle(request);
			switch ( response ) {
				case GetStateResponse.Found<TState> found: {
					State = found.State;
					_logger.LogTrace("State found");
					break;
				}

				case object value: {
					_logger.LogError($"Result is '{value.GetType()}'");
					throw new InvalidOperationException();
				}
			}
		}
	}
}
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Client.Abstractions;
using Core.Client.Shared;
using Core.Common.Command;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;
using Core.Common.Threading;
using Core.Common.Utils;
using Core.Service.Model;
using Core.Service.Queue;
using Core.Service.UseCase.GetConfig;
using Core.Service.UseCase.GetState;
using Core.Service.UseCase.SendCommand;
using Core.Service.UseCase.WaitCommand;

namespace Core.Client.Embedded {
	public sealed class EmbeddedServiceWaitClient<TConfig, TState> : IClient<TConfig, TState>
		where TConfig : IConfig where TState : class, IState, new() {
		readonly UserId _userId;

		readonly ILogger<EmbeddedServiceWaitClient<TConfig, TState>> _logger;
		readonly CommandExecutor<TConfig, TState>                    _singleExecutor;

		readonly GetConfigUseCase<TConfig>           _getConfigUseCase;
		readonly GetStateUseCase<TState>             _getStateUseCase;
		readonly SendCommandUseCase<TConfig, TState> _sendCommandUseCase;
		readonly WaitCommandUseCase<TConfig, TState> _waitCommandUseCase;
		readonly CommandScheduler<TConfig, TState>   _scheduler;
		readonly ITaskRunner                         _taskRunner;

		public TState  State  { get; private set; }
		public TConfig Config { get; private set; }

		public EmbeddedServiceWaitClient(
			ILoggerFactory                      loggerFactory,
			CommandExecutor<TConfig, TState>    commandExecutor,
			GetConfigUseCase<TConfig>           getConfigUseCase,
			GetStateUseCase<TState>             getStateUseCase,
			SendCommandUseCase<TConfig, TState> sendCommandUseCase,
			WaitCommandUseCase<TConfig, TState> waitCommandUseCase,
			UserIdSource                        userIdSource,
			CommandScheduler<TConfig, TState>   scheduler,
			ITaskRunner                         taskRunner) {
			_userId             = userIdSource.GetOrCreateUserId();
			_logger             = loggerFactory.Create<EmbeddedServiceWaitClient<TConfig, TState>>();
			_getConfigUseCase   = getConfigUseCase;
			_getStateUseCase    = getStateUseCase;
			_sendCommandUseCase = sendCommandUseCase;
			_waitCommandUseCase = waitCommandUseCase;
			_singleExecutor     = commandExecutor;
			_scheduler          = scheduler;
			_taskRunner         = taskRunner;
		}

		public async Task<InitializationResult> Initialize(CancellationToken cancellationToken) {
			try {
				await UpdateConfig();
				await UpdateState();
				_taskRunner.Run(WaitCommand, cancellationToken);
				_taskRunner.Run(ProcessCommand, cancellationToken);
			} catch ( Exception e ) {
				return new InitializationResult.Error(e.ToString());
			}
			return new InitializationResult.Ok();
		}

		public async Task<CommandApplyResult> Apply(ICommand<TConfig, TState> command, CancellationToken cancellationToken) {
			var request  = new SendCommandRequest<TConfig, TState>(_userId, State.Version, Config.Version, command);
			var response = await _sendCommandUseCase.Handle(request);
			cancellationToken.ThrowIfCancellationRequested();
			switch ( response ) {
				case SendCommandResponse.Applied _: {
					return new CommandApplyResult.Ok();
				}

				case SendCommandResponse.Rejected rejected: {
					return new CommandApplyResult.Error($"Command rejected: '{rejected.Description}'");
				}

				case SendCommandResponse.BadRequest badRequest: {
					return new CommandApplyResult.Error($"Command is invalid: '{badRequest.Description}'");
				}

				case object value: {
					return new CommandApplyResult.Error($"Result is '{value.GetType()}', force update state");
				}

				default: {
					return new CommandApplyResult.Error("Failed to apply command");
				}
			}
		}

		async Task WaitCommand(CancellationToken cancellationToken) {
			while ( true ) {
				_logger.LogTrace("Awaiting for new command");
				var request = new WaitCommandRequest(_userId, State.Version, Config.Version);
				var response = await _waitCommandUseCase.Handle(request);
				cancellationToken.ThrowIfCancellationRequested();
				switch ( response ) {
					case WaitCommandResponse.Updated<TConfig, TState> updated: {
						_logger.LogTrace($"New commands found: {updated.NextCommands.Length}");
						foreach ( var cmd in updated.NextCommands ) {
							await _singleExecutor.Apply(Config, State, cmd, true, cancellationToken);
						}
						foreach ( var error in updated.Errors ) {
							_logger.LogError(error.ToString());
						}
						State.Version = updated.NewVersion;
						break;
					}

					case WaitCommandResponse.NotFound _: {
						_logger.LogTrace("No new commands found");
						break;
					}

					case WaitCommandResponse.Outdated _: {
						_logger.LogTrace($"Command outdated");
						break;
					}

					case WaitCommandResponse.BadRequest badRequest: {
						_logger.LogTrace($"Command failed: '{badRequest.Description}'");
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

		async Task ProcessCommand(CancellationToken cancellationToken) {
			while ( true ) {
				await _scheduler.Update();
				cancellationToken.ThrowIfCancellationRequested();
				await _taskRunner.Delay(TimeSpan.FromSeconds(1));
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
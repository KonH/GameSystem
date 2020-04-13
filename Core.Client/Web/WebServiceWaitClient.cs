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
using Core.Service.UseCase.GetConfig;
using Core.Service.UseCase.GetState;
using Core.Service.UseCase.UpdateState;
using Core.Service.UseCase.WaitCommand;

namespace Core.Client.Web {
	public sealed class WebServiceWaitClient<TConfig, TState> : IClient<TConfig, TState>
		where TConfig : IConfig where TState : class, IState {
		readonly UserId _userId;

		readonly ILogger<WebServiceWaitClient<TConfig, TState>> _logger;
		readonly CommandExecutor<TConfig, TState>               _singleExecutor;
		readonly WebClientHandler                               _webClientHandler;
		readonly ITaskRunner                                    _taskRunner;

		bool _isUpdatingState = false;

		public TState  State  { get; private set; }
		public TConfig Config { get; private set; }

		public WebServiceWaitClient(
			ILoggerFactory loggerFactory, CommandExecutor<TConfig, TState> commandExecutor,
			WebClientHandler webClientHandler, UserIdSource userIdSource, ITaskRunner taskRunner) {
			_userId           = userIdSource.GetOrCreateUserId();
			_logger           = loggerFactory.Create<WebServiceWaitClient<TConfig, TState>>();
			_singleExecutor   = commandExecutor;
			_webClientHandler = webClientHandler;
			_taskRunner       = taskRunner;
		}

		public async Task<InitializationResult> Initialize(CancellationToken cancellationToken) {
			try {
				await UpdateConfig();
				await UpdateState();
				_taskRunner.Run(WaitCommand, cancellationToken);
			} catch ( Exception e ) {
				return new InitializationResult.Error(e.ToString());
			}
			return new InitializationResult.Ok();
		}

		public async Task<CommandApplyResult> Apply(ICommand<TConfig, TState> command) {
			var request  = new UpdateStateRequest<TConfig, TState>(_userId, State.Version, Config.Version, command);
			_isUpdatingState = true;
			UpdateStateResponse response;
			try {
				response = await _webClientHandler.UpdateState(request);
			} finally {
				_isUpdatingState = false;
			}
			switch ( response ) {
				case UpdateStateResponse.Updated<TConfig, TState> updated: {
					await _singleExecutor.Apply(Config, State, command, true);
					foreach ( var cmd in updated.NextCommands ) {
						await _singleExecutor.Apply(Config, State, cmd, true);
					}
					State.Version = updated.NewVersion;
					return new CommandApplyResult.Ok();
				}

				case UpdateStateResponse.Rejected rejected: {
					return new CommandApplyResult.Error($"Command rejected: '{rejected.Description}'");
				}

				case UpdateStateResponse.BadRequest badRequest: {
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

		async Task WaitCommand() {
			while ( true ) {
				while ( _isUpdatingState ) {
					await _taskRunner.Delay(TimeSpan.FromSeconds(1));
				}
				_logger.LogTrace("Awaiting for new command");
				var request  = new WaitCommandRequest(_userId, State.Version, Config.Version);
				var response = await _webClientHandler.WaitCommand(request);
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

					case WaitCommandResponse.Outdated _: {
						_logger.LogTrace($"Command outdated");
						break;
					}

					case WaitCommandResponse.Rejected rejected: {
						_logger.LogTrace($"Command rejected: '{rejected.Description}'");
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

		async Task UpdateConfig() {
			_logger.LogTrace($"Update config for '{_userId}'");
			var request  = new GetConfigRequest(_userId);
			var response = await _webClientHandler.GetConfig(request);
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
			var response = await _webClientHandler.GetState(request);
			switch ( response ) {
				case GetStateResponse.Found<TState> found: {
					State = found.State;
					_logger.LogTrace("State found");
					break;
				}

				case GetStateResponse.BadRequest badRequest: {
					_logger.LogError($"Failed to get state: '{badRequest.Description}'");
					throw new InvalidOperationException();
				}

				case object value: {
					_logger.LogError($"Result is '{value.GetType()}'");
					throw new InvalidOperationException();
				}
			}
		}
	}
}
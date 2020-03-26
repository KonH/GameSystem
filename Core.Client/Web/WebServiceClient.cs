using System;
using System.Threading.Tasks;
using Core.Client.Abstractions;
using Core.Client.Shared;
using Core.Common.Command;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;
using Core.Common.Utils;
using Core.Service.Model;
using Core.Service.UseCase.GetConfig;
using Core.Service.UseCase.GetState;
using Core.Service.UseCase.UpdateState;

namespace Core.Client.Web {
	public sealed class WebServiceClient<TConfig, TState> : IClient<TConfig, TState>
		where TConfig : IConfig where TState : class, IState {
		readonly UserId _userId;

		readonly ILogger<WebServiceClient<TConfig, TState>> _logger;
		readonly CommandExecutor<TConfig, TState>           _singleExecutor;
		readonly WebClientHandler                           _webClientHandler;

		public TState  State  { get; private set; }
		public TConfig Config { get; private set; }

		public WebServiceClient(
			ILoggerFactory loggerFactory, CommandExecutor<TConfig, TState> commandExecutor,
			WebClientHandler webClientHandler, UserIdSource userIdSource) {
			_userId           = userIdSource.GetOrCreateUserId();
			_logger           = loggerFactory.Create<WebServiceClient<TConfig, TState>>();
			_singleExecutor   = commandExecutor;
			_webClientHandler = webClientHandler;
		}

		public async Task<InitializationResult> Initialize() {
			try {
				await UpdateConfig();
				await UpdateState();
			} catch ( Exception e ) {
				return new InitializationResult.Error(e.ToString());
			}
			return new InitializationResult.Ok();
		}

		public async Task<CommandApplyResult> Apply(ICommand<TConfig, TState> command) {
			var request  = new UpdateStateRequest<TConfig, TState>(_userId, State.Version, Config.Version, command);
			var response = await _webClientHandler.UpdateState(request);
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
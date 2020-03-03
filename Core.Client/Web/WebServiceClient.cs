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
		public TState State { get; private set; }

		readonly UserId _userId = new UserId("UserId");

		readonly ILogger<WebServiceClient<TConfig, TState>> _logger;
		readonly CommandExecutor<TConfig, TState>           _singleExecutor;
		readonly WebClientHandler                           _webClientHandler;

		TConfig _config;

		public WebServiceClient(
			ILoggerFactory loggerFactory, CommandExecutor<TConfig, TState> commandExecutor,
			WebClientHandler webClientHandler) {
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
			var request  = new UpdateStateRequest<TConfig, TState>(_userId, State.Version, _config.Version, command);
			var response = await _webClientHandler.UpdateState(request);
			switch ( response ) {
				case UpdateStateResponse.Updated<TConfig, TState> updated: {
					await _singleExecutor.Apply(_config, State, command, true);
					foreach ( var cmd in updated.NextCommands ) {
						await _singleExecutor.Apply(_config, State, cmd, true);
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

		async Task UpdateConfig() {
			_logger.LogTrace($"Update config for '{_userId}'");
			var request  = new GetConfigRequest(_userId);
			var response = await _webClientHandler.GetConfig(request);
			switch ( response ) {
				case GetConfigResponse.Found<TConfig> found: {
					_config = found.Config;
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

				case object value: {
					_logger.LogError($"Result is '{value.GetType()}'");
					throw new InvalidOperationException();
				}
			}
		}
	}
}
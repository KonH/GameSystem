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

namespace Core.Client.Embedded {
	public sealed class EmbeddedServiceClient<TConfig, TState> : IClient<TConfig, TState>
		where TConfig : IConfig where TState : class, IState {
		readonly UserId _userId = new UserId("UserId");

		readonly ILogger<EmbeddedServiceClient<TConfig, TState>> _logger;
		readonly CommandExecutor<TConfig, TState>                _singleExecutor;

		readonly GetConfigUseCase<TConfig>           _getConfigUseCase;
		readonly GetStateUseCase<TState>             _getStateUseCase;
		readonly UpdateStateUseCase<TConfig, TState> _updateStateUseCase;

		public TState  State  { get; private set; }
		public TConfig Config { get; private set; }

		public EmbeddedServiceClient(
			ILoggerFactory loggerFactory,
			CommandExecutor<TConfig, TState> commandExecutor,
			GetConfigUseCase<TConfig> getConfigUseCase,
			GetStateUseCase<TState> getStateUseCase,
			UpdateStateUseCase<TConfig, TState> updateStateUseCase) {
			_logger             = loggerFactory.Create<EmbeddedServiceClient<TConfig, TState>>();
			_getConfigUseCase   = getConfigUseCase;
			_getStateUseCase    = getStateUseCase;
			_updateStateUseCase = updateStateUseCase;
			_singleExecutor     = commandExecutor;
		}

		public Task<InitializationResult> Initialize() {
			try {
				UpdateConfig();
				UpdateState();
			} catch ( Exception e ) {
				return Task.FromResult<InitializationResult>(
					new InitializationResult.Error(e.ToString()));
			}
			return Task.FromResult<InitializationResult>(
				new InitializationResult.Ok());
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

		void UpdateConfig() {
			_logger.LogTrace($"Update config for '{_userId}'");
			var request  = new GetConfigRequest(_userId);
			var response = _getConfigUseCase.Handle(request);
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

		void UpdateState() {
			_logger.LogTrace($"Update state for '{_userId}'");
			var request  = new GetStateRequest(_userId);
			var response = _getStateUseCase.Handle(request);
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
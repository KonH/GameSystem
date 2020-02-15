using System;
using Core.Common.Command;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;
using Core.Common.Utils;
using Core.Service.Model;
using Core.Service.UseCase.GetConfig;
using Core.Service.UseCase.GetState;
using Core.Service.UseCase.UpdateState;

namespace Core.Client {
	public sealed class EmbeddedServiceClient<TConfig, TState> : SyncClient<TConfig, TState>
		where TConfig : IConfig where TState : class, IState {
		readonly UserId _userId = new UserId("UserId");

		readonly ILogger<EmbeddedServiceClient<TConfig, TState>> _logger;
		readonly CommandExecutor<TConfig, TState>                _singleExecutor;

		readonly GetConfigUseCase<TConfig>           _getConfigUseCase;
		readonly GetStateUseCase<TState>             _getStateUseCase;
		readonly UpdateStateUseCase<TConfig, TState> _updateStateUseCase;

		TConfig _config;

		public EmbeddedServiceClient(
			ILoggerFactory loggerFactory,
			GetConfigUseCase<TConfig> getConfigUseCase,
			GetStateUseCase<TState> getStateUseCase,
			UpdateStateUseCase<TConfig, TState> updateStateUseCase) {
			_logger             = loggerFactory.Create<EmbeddedServiceClient<TConfig, TState>>();
			_getConfigUseCase   = getConfigUseCase;
			_getStateUseCase    = getStateUseCase;
			_updateStateUseCase = updateStateUseCase;
			_singleExecutor     = new CommandExecutor<TConfig, TState>();
		}

		protected override InitializationResult Initialize() {
			try {
				UpdateConfig();
				UpdateState();
			} catch ( Exception e ) {
				return new InitializationResult.Error(e.ToString());
			}
			return new InitializationResult.Ok();
		}

		protected override CommandApplyResult Apply(ICommand<TConfig, TState> command) {
			var request  = new UpdateStateRequest<TConfig, TState>(_userId, State.Version, _config.Version, command);
			var response = _updateStateUseCase.Handle(request);
			switch ( response ) {
				case UpdateStateResponse.Updated<TConfig, TState> updated: {
					_singleExecutor.Apply(_config, State, command);
					foreach ( var cmd in updated.NextCommands ) {
						_singleExecutor.Apply(_config, State, cmd);
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
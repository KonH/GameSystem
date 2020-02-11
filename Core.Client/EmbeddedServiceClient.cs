using System;
using Core.Common.Command;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;
using Core.Common.Utils;
using Core.Service.Extension;
using Core.Service.Model;
using Core.Service.Repository.Config;
using Core.Service.Repository.State;
using Core.Service.UseCase.GetState;
using Core.Service.UseCase.UpdateState;

namespace Core.Client {
	public sealed class EmbeddedServiceClient<TConfig, TState> : IClient<TConfig, TState>
		where TConfig : IConfig where TState : class, IState {
		public TState State { get; private set; }

		readonly UserId _userId = new UserId("UserId");

		readonly ILogger<EmbeddedServiceClient<TConfig, TState>> _logger;
		readonly CommandExecutor<TConfig, TState>                _singleExecutor;
		readonly TConfig                                         _config;
		readonly StateFactory<TState>                            _stateFactory;

		readonly IConfigRepository<TConfig>          _configRepository;
		readonly IStateRepository<TState>            _stateRepository;
		readonly GetStateUseCase<TState>             _getStateUseCase;
		readonly UpdateStateUseCase<TConfig, TState> _updateStateUseCase;

		public EmbeddedServiceClient(
			LoggerFactory loggerFactory, TConfig config, StateFactory<TState> stateFactory,
			IConfigRepository<TConfig> configRepository, IStateRepository<TState> stateRepository,
			GetStateUseCase<TState> getStateUseCase, UpdateStateUseCase<TConfig, TState> updateStateUseCase) {
			_logger             = loggerFactory.Create<EmbeddedServiceClient<TConfig, TState>>();
			_config             = config;
			_stateFactory       = stateFactory;
			_configRepository   = configRepository;
			_stateRepository    = stateRepository;
			_getStateUseCase    = getStateUseCase;
			_updateStateUseCase = updateStateUseCase;
			_singleExecutor     = new CommandExecutor<TConfig, TState>();
		}

		public InitializationResult Initialize() {
			_configRepository.Add(_config);
			_stateRepository.AddForUserId(_userId, _stateFactory.Create());
			UpdateState();
			return new InitializationResult.Ok();
		}

		public CommandApplyResult Apply(ICommand<TConfig, TState> command) {
			var request  = new UpdateStateRequest<TConfig, TState>(_userId, State.Version, _config.Version, command);
			var response = _updateStateUseCase.Handle(request);
			switch ( response ) {
				case UpdateStateResponse<TConfig, TState>.Updated updated: {
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

		void UpdateState() {
			_logger.LogTrace($"Update state for '{_userId}'");
			var request  = new GetStateRequest(_userId);
			var response = _getStateUseCase.Handle(request);
			switch ( response ) {
				case GetStateResponse<TState>.Found found: {
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
using System.Collections.Generic;
using Core.Common.Command;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;
using Core.Service.Extension;
using Core.Service.Model;
using Core.Service.Repository.Config;
using Core.Service.Repository.State;

namespace Core.Service.UseCase.UpdateState {
	public sealed class
		UpdateStateUseCase<TConfig, TState> :
			IUseCase<UpdateStateRequest<TConfig, TState>, UpdateStateResponse<TConfig, TState>>
		where TConfig : IConfig where TState : IState {
		readonly IStateRepository<TState>              _stateRepository;
		readonly IConfigRepository<TConfig>            _configRepository;
		readonly BatchCommandExecutor<TConfig, TState> _commandExecutor;

		public UpdateStateUseCase(
			IStateRepository<TState> stateRepository, IConfigRepository<TConfig> configRepository,
			BatchCommandExecutor<TConfig, TState> commandExecutor) {
			_stateRepository  = stateRepository;
			_configRepository = configRepository;
			_commandExecutor  = commandExecutor;
		}

		public UpdateStateResponse<TConfig, TState> Handle(UpdateStateRequest<TConfig, TState> request) {
			var validateError = Validate(request, out var config, out var state);
			if ( validateError != null ) {
				return validateError;
			}
			var result = _commandExecutor.Apply(config, state, request.Command);
			return HandleResult(request.UserId, state, result);
		}

		UpdateStateResponse<TConfig, TState> Validate(
			UpdateStateRequest<TConfig, TState> request, out TConfig config, out TState state) {
			state  = default;
			config = default;
			if ( request == null ) {
				return BadRequest("null request");
			}
			if ( request.Command == null ) {
				return BadRequest("null command");
			}
			state = _stateRepository.GetByUserId(request.UserId);
			if ( state == null ) {
				return NotFound();
			}
			if ( state.Version > request.StateVersion ) {
				return Outdated();
			}
			config = _configRepository.Get(request.ConfigVersion);
			if ( config == null ) {
				return BadRequest($"Config '{request.ConfigVersion}' isn't found");
			}
			return null;
		}

		UpdateStateResponse<TConfig, TState> HandleResult(
			UserId userId, TState state, BatchCommandResult<TConfig, TState> result) {
			switch ( result ) {
				case BatchCommandResult<TConfig, TState>.Ok okResult: {
					_stateRepository.UpdateForUserId(userId, state);
					var newVersion = state.Version;
					return Updated(newVersion, okResult.NextCommands);
				}

				case BatchCommandResult<TConfig, TState>.BadCommand badResult: {
					return Rejected(badResult.Description);
				}

				default: {
					return BadRequest();
				}
			}
		}

		static UpdateStateResponse<TConfig, TState> NotFound() {
			return new UpdateStateResponse<TConfig, TState>.NotFound();
		}

		static UpdateStateResponse<TConfig, TState> Outdated() {
			return new UpdateStateResponse<TConfig, TState>.Outdated();
		}

		static UpdateStateResponse<TConfig, TState> Updated(
			StateVersion newVersion, List<ICommand<TConfig, TState>> nextCommands) {
			return new UpdateStateResponse<TConfig, TState>.Updated(newVersion, nextCommands);
		}

		static UpdateStateResponse<TConfig, TState> Rejected(string description) {
			return new UpdateStateResponse<TConfig, TState>.Rejected(description);
		}

		static UpdateStateResponse<TConfig, TState> BadRequest(string description = "") {
			return new UpdateStateResponse<TConfig, TState>.BadRequest(description);
		}
	}
}
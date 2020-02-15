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
			IUseCase<UpdateStateRequest<TConfig, TState>, UpdateStateResponse>
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

		public UpdateStateResponse Handle(UpdateStateRequest<TConfig, TState> request) {
			var validateError = Validate(request, out var config, out var state);
			if ( validateError != null ) {
				return validateError;
			}
			var result = _commandExecutor.Apply(config, state, request.Command);
			return HandleResult(request.UserId, state, result);
		}

		UpdateStateResponse Validate(
			UpdateStateRequest<TConfig, TState> request, out TConfig config, out TState state) {
			state  = default;
			config = default;
			if ( request == null ) {
				return BadRequest("null request");
			}
			if ( request.Command == null ) {
				return BadRequest("null command");
			}
			state = _stateRepository.Get(request.UserId);
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

		UpdateStateResponse HandleResult(
			UserId userId, TState state, BatchCommandResult result) {
			switch ( result ) {
				case BatchCommandResult.Ok<TConfig, TState> okResult: {
					_stateRepository.Update(userId, state);
					var newVersion = state.Version;
					return Updated(newVersion, okResult.NextCommands);
				}

				case BatchCommandResult.BadCommand badResult: {
					return Rejected(badResult.Description);
				}

				default: {
					return BadRequest();
				}
			}
		}

		static UpdateStateResponse NotFound() {
			return new UpdateStateResponse.NotFound();
		}

		static UpdateStateResponse Outdated() {
			return new UpdateStateResponse.Outdated();
		}

		static UpdateStateResponse Updated(
			StateVersion newVersion, List<ICommand<TConfig, TState>> nextCommands) {
			return new UpdateStateResponse.Updated<TConfig, TState>(newVersion, nextCommands);
		}

		static UpdateStateResponse Rejected(string description) {
			return new UpdateStateResponse.Rejected(description);
		}

		static UpdateStateResponse BadRequest(string description = "") {
			return new UpdateStateResponse.BadRequest(description);
		}
	}
}
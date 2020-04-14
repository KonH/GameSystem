using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Common.Command;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;
using Core.Common.Threading;
using Core.Service.Extension;
using Core.Service.Model;
using Core.Service.Queue;
using Core.Service.Repository.Config;
using Core.Service.Repository.State;

namespace Core.Service.UseCase.WaitCommand {
	public sealed class
		WaitCommandUseCase<TConfig, TState> : IUseCase<WaitCommandRequest, WaitCommandResponse>
		where TConfig : IConfig where TState : class, IState, new() {
		readonly WaitCommandSettings                   _settings;
		readonly CommandAwaiter<TConfig, TState>       _awaiter;
		readonly IStateRepository<TState>              _stateRepository;
		readonly IConfigRepository<TConfig>            _configRepository;
		readonly BatchCommandExecutor<TConfig, TState> _commandExecutor;
		readonly ITaskRunner                           _taskRunner;

		public WaitCommandUseCase(
			WaitCommandSettings settings, CommandAwaiter<TConfig, TState> awaiter,
			IStateRepository<TState> stateRepository, IConfigRepository<TConfig> configRepository,
			BatchCommandExecutor<TConfig, TState> commandExecutor, ITaskRunner taskRunner) {
			_settings         = settings;
			_awaiter          = awaiter;
			_stateRepository  = stateRepository;
			_configRepository = configRepository;
			_commandExecutor  = commandExecutor;
			_taskRunner       = taskRunner;
		}

		public async Task<WaitCommandResponse> Handle(WaitCommandRequest request) {
			var (validateError, (config, state)) = await Validate(request);
			if ( validateError != null ) {
				return validateError;
			}
			var commandTask = _awaiter.WaitForCommands(request.UserId, config, state);
			var delayTask   = _taskRunner.Delay(_settings.WaitTime);
			await Task.WhenAny(commandTask, delayTask);
			_awaiter.CancelWaiting(request.UserId);
			if ( commandTask.IsCompleted ) {
				var actualState = await _stateRepository.Get(request.UserId);
				if ( actualState.Version > state.Version ) {
					return Outdated();
				}
				var commands = commandTask.Result;
				var allCommands = new List<ICommand<TConfig, TState>>(commands.Length);
				var lastVersion = state.Version;
				foreach ( var command in commands ) {
					var result = await _commandExecutor.Apply(config, state, command, false, CancellationToken.None);
					var response = HandleResult(request.UserId, command, state, result);
					if ( response is WaitCommandResponse.Updated<TConfig, TState> okResponse ) {
						allCommands.AddRange(okResponse.NextCommands);
						lastVersion = okResponse.NewVersion;
						continue;
					}
					return response;
				}
				return Updated(lastVersion, allCommands);
			}
			return NotFound();
		}

		async Task<(WaitCommandResponse failed, (TConfig config, TState state))> Validate(WaitCommandRequest request) {
			(WaitCommandResponse, (TConfig, TState)) Failed(WaitCommandResponse error) {
				return (error, (default, default));
			}
			if ( request == null ) {
				return Failed(BadRequest("null request"));
			}
			var state = await _stateRepository.Get(request.UserId) ?? new TState();
			if ( state.Version > request.StateVersion ) {
				return Failed(Outdated());
			}
			var config = await _configRepository.Get(request.ConfigVersion);
			if ( config == null ) {
				return Failed(BadRequest($"Config '{request.ConfigVersion}' isn't found"));
			}
			return (null, (config, state));
		}

		WaitCommandResponse HandleResult(
			UserId userId, ICommand<TConfig, TState> command, TState state, BatchCommandResult result) {
			switch ( result ) {
				case BatchCommandResult.Ok<TConfig, TState> okResult: {
					_stateRepository.Update(userId, state);
					var newVersion = state.Version;
					var nextCommands = okResult.NextCommands;
					var allCommands = new List<ICommand<TConfig, TState>> {
						command
					};
					allCommands.AddRange(nextCommands);
					return Updated(newVersion, allCommands);
				}

				case BatchCommandResult.BadCommand badResult: {
					return Rejected(badResult.Description);
				}

				default: {
					return BadRequest();
				}
			}
		}

		static WaitCommandResponse Outdated() {
			return new WaitCommandResponse.Outdated();
		}

		static WaitCommandResponse Updated(
			StateVersion newVersion, List<ICommand<TConfig, TState>> nextCommands) {
			return new WaitCommandResponse.Updated<TConfig, TState>(newVersion, nextCommands);
		}

		static WaitCommandResponse Rejected(string description) {
			return new WaitCommandResponse.Rejected(description);
		}

		static WaitCommandResponse NotFound() {
			return new WaitCommandResponse.NotFound();
		}

		static WaitCommandResponse BadRequest(string description = "") {
			return new WaitCommandResponse.BadRequest(description);
		}
	}
}
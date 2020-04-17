using System.Threading.Tasks;
using Core.Common.Command;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;
using Core.Common.Threading;
using Core.Service.Extension;
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
		readonly ITaskRunner                           _taskRunner;

		public WaitCommandUseCase(
			WaitCommandSettings      settings,        CommandAwaiter<TConfig, TState> awaiter,
			IStateRepository<TState> stateRepository, IConfigRepository<TConfig>      configRepository,
			ITaskRunner              taskRunner) {
			_settings         = settings;
			_awaiter          = awaiter;
			_stateRepository  = stateRepository;
			_configRepository = configRepository;
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
				var result = commandTask.Result;
				var lastVersion = actualState.Version;
				return Updated(lastVersion, result.Commands, result.Errors);
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

		static WaitCommandResponse Outdated() {
			return new WaitCommandResponse.Outdated();
		}

		static WaitCommandResponse Updated(
			StateVersion newVersion, ICommand<TConfig, TState>[] nextCommands, BatchCommandResult[] errors) {
			return new WaitCommandResponse.Updated<TConfig, TState>(newVersion, nextCommands, errors);
		}

		static WaitCommandResponse NotFound() {
			return new WaitCommandResponse.NotFound();
		}

		static WaitCommandResponse BadRequest(string description = "") {
			return new WaitCommandResponse.BadRequest(description);
		}
	}
}
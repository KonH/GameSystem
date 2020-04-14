using System.Threading;
using System.Threading.Tasks;
using Core.Client.Abstractions;
using Core.Client.Shared;
using Core.Common.Command;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Client.Standalone {
	public sealed class StandaloneClient<TConfig, TState> : IClient<TConfig, TState>
		where TConfig : IConfig where TState : IState {
		readonly CommandExecutor<TConfig, TState>      _singleExecutor;
		readonly BatchCommandExecutor<TConfig, TState> _batchExecutor;
		readonly StateFactory<TState>                  _stateFactory;

		CommandHistory<TConfig, TState> _history = new CommandHistory<TConfig, TState>();

		public TState  State  { get; private set; }
		public TConfig Config { get; private set; }

		public StandaloneClient(
			CommandExecutor<TConfig, TState> commandExecutor,
			BatchCommandExecutor<TConfig, TState> batchExecutor,
			TConfig config, StateFactory<TState> stateFactory) {
			_stateFactory   = stateFactory;
			_singleExecutor = commandExecutor;
			_batchExecutor  = batchExecutor;
			Config          = config;
			State           = _stateFactory.Create();
		}

		public Task<InitializationResult> Initialize(CancellationToken cancellationToken) =>
			Task.FromResult<InitializationResult>(new InitializationResult.Ok());

		public async Task<CommandApplyResult> Apply(ICommand<TConfig, TState> command, CancellationToken cancellationToken) {
			var result = await _batchExecutor.Apply(Config, State, command, true);
			switch ( result ) {
				case BatchCommandResult.Ok<TConfig, TState> okResult: {
					_history.AddCommand(command, true);
					_history.AddCommands(okResult.NextCommands, true);
					return new CommandApplyResult.Ok();
				}

				default: {
					_history.AddCommand(command, false);
					Rewind();
					return new CommandApplyResult.Error("Failed to apply command");
				}
			}
		}

		void Rewind() {
			State = _stateFactory.Create();
			var validCommands = _history.ValidCommands;
			_history = new CommandHistory<TConfig, TState>();
			foreach ( var command in validCommands ) {
				_singleExecutor.Apply(Config, State, command);
			}
			_history.AddCommands(validCommands, true);
		}
	}
}
using System;
using Core.Common.Command;
using Core.Common.CommandDependency;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;
using Core.Common.Utils;

namespace Core.Client {
	public sealed class StandaloneClient<TConfig, TState>
		where TConfig : IConfig where TState : IState {
		public TState State { get; private set; }

		readonly CommandExecutor<TConfig, TState>      _singleExecutor;
		readonly BatchCommandExecutor<TConfig, TState> _batchExecutor;
		readonly TConfig                               _config;
		readonly Func<TState>                          _stateInitializer;

		CommandHistory<TConfig, TState> _history = new CommandHistory<TConfig, TState>();

		public StandaloneClient(
			LoggerFactory loggerFactory, CommandQueue<TConfig, TState> queue,
			TConfig config, Func<TState> state) {
			_config = config;
			_stateInitializer = state;
			_singleExecutor = new CommandExecutor<TConfig, TState>();
			var logger = loggerFactory.Create<BatchCommandExecutor<TConfig, TState>>();
			_batchExecutor = new BatchCommandExecutor<TConfig, TState>(logger, queue);
			State = state();
		}

		public BatchCommandResult<TConfig, TState> Apply(ICommand<TConfig, TState> command) {
			var result = _batchExecutor.Apply(_config, State, command);
			switch ( result ) {
				case BatchCommandResult<TConfig, TState>.OkResult okResult: {
					_history.AddCommand(command, true);
					_history.AddCommands(okResult.NextCommands, true);
					break;
				}

				default: {
					_history.AddCommand(command, false);
					break;
				}
			}
			return result;
		}

		public void Rewind() {
			State = _stateInitializer();
			var validCommands = _history.ValidCommands;
			_history = new CommandHistory<TConfig, TState>();
			foreach ( var command in validCommands ) {
				_singleExecutor.Apply(_config, State, command);
			}
			_history.AddCommands(validCommands, true);
		}
	}
}
using System.Collections.Generic;
using System.Linq;
using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Client.Standalone {
	sealed class CommandHistory<TConfig, TState> where TConfig : IConfig where TState : IState {
		readonly List<ICommand<TConfig, TState>> _commands = new List<ICommand<TConfig, TState>>();

		int _validCount = 0;

		public IReadOnlyCollection<ICommand<TConfig, TState>> AllCommands => _commands;

		public IReadOnlyCollection<ICommand<TConfig, TState>> ValidCommands => _commands.Take(_validCount).ToArray();

		public void AddCommand(ICommand<TConfig, TState> command, bool valid) {
			_commands.Add(command);
			if ( valid ) {
				_validCount = _commands.Count;
			}
		}

		public void AddCommands(IReadOnlyCollection<ICommand<TConfig, TState>> commands, bool valid) {
			_commands.AddRange(commands);
			if ( valid ) {
				_validCount = _commands.Count;
			}
		}
	}
}
using System.Collections.Generic;
using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Service.Queue {
	public sealed class CommandSet<TConfig, TState> where TConfig: IConfig where TState : IState {
		readonly List<ICommand<TConfig, TState>> _commands = new List<ICommand<TConfig, TState>>();

		public bool IsEmpty => _commands.Count == 0;

		public void Add(ICommand<TConfig, TState> command) {
			_commands.Add(command);
		}

		internal ICommand<TConfig, TState>[] ToArray() {
			var result = _commands.ToArray();
			_commands.Clear();
			return result;
		}
	}
}
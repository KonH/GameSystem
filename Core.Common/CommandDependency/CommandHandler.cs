using System.Collections.Generic;
using System.Linq;
using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Common.CommandDependency {
	public sealed class CommandHandler<TConfig, TState> where TConfig : IConfig where TState : IState {
		readonly CommandQueue<TConfig, TState> _queue;

		readonly IReadOnlyCollection<ICommand<TConfig, TState>> _noDependencies = new ICommand<TConfig, TState>[0];

		public CommandHandler(CommandQueue<TConfig, TState> queue) {
			_queue = queue;
		}

		public IReadOnlyCollection<ICommand<TConfig, TState>> GetDependentCommands(
			TConfig config, TState state, ICommand<TConfig, TState> command) {
			var commandType = command.GetType();
			var dependencies = _queue.Dependencies.GetValueOrDefault(commandType);
			if ( dependencies == null ) {
				return _noDependencies;
			}
			return dependencies
				.Select(d => {
					if ( !d.Condition(config, state, command) ) {
						return null;
					}
					return d.Initializer(config, state, command);
				})
				.Where(d => d != null)
				.ToArray();
		}
	}
}
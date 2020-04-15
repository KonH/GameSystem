using System.Collections.Concurrent;
using System.Collections.Generic;
using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;
using Core.Service.Model;

namespace Core.Service.Queue {
	public sealed class CommonWatcher<TConfig, TState> : IUpdateWatcher<TConfig, TState> where TConfig : IConfig where TState : IState {
		readonly ConcurrentDictionary<UserId, List<ICommand<TConfig, TState>>> _commands =
			new ConcurrentDictionary<UserId, List<ICommand<TConfig, TState>>>();

		public void AddCommand(UserId userId, ICommand<TConfig, TState> command) {
			_commands.AddOrUpdate(
				userId,
				id => new List<ICommand<TConfig, TState>> { command },
				(id, commands) => {
					lock ( id ) {
						commands.Add(command);
					}
					return commands;
				});
		}

		public void TryAddCommands(UserId userId, TConfig config, TState state, CommandSet<TConfig, TState> commands) {
			if ( _commands.TryGetValue(userId, out var userCommands) ) {
				foreach ( var command in userCommands ) {
					commands.Add(command);
				}
				userCommands.Clear();
			}
		}
	}
}
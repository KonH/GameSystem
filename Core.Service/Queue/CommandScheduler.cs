using System.Collections.Generic;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Service.Queue {
	public sealed class CommandScheduler<TConfig, TState> where TConfig : IConfig where TState : IState {
		public sealed class Settings {
			public IReadOnlyCollection<IUpdateWatcher<TConfig, TState>> Watchers => _watchers.AsReadOnly();

			List<IUpdateWatcher<TConfig, TState>> _watchers = new List<IUpdateWatcher<TConfig, TState>>();

			public void AddWatcher(IUpdateWatcher<TConfig, TState> watcher) {
				_watchers.Add(watcher);
			}
		}

		static readonly CommandSet<TConfig, TState> _commands = new CommandSet<TConfig, TState>();

		readonly Settings                          _settings;
		readonly CommandWorkQueue<TConfig, TState> _queue;

		public CommandScheduler(Settings settings, CommandWorkQueue<TConfig, TState> queue) {
			_settings = settings;
			_queue    = queue;
		}

		public void Update() {
			_queue.ForEachPending((userId, item) => {
				foreach ( var watcher in _settings.Watchers ) {
					watcher.TryAddCommands(userId, item.Config, item.State, _commands);
				}
				if ( !_commands.IsEmpty ) {
					var commands = _commands.ToArray();
					item.Complete(commands);
				}
			});
		}
	}
}
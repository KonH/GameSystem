using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;
using Core.Service.Model;

namespace Core.Service {
	public sealed class CommandScheduler<TConfig, TState> where TConfig : IConfig where TState : IState {
		public sealed class Settings {
			public IReadOnlyCollection<IUpdateWatcher<TConfig, TState>> Watchers => _watchers.AsReadOnly();

			List<IUpdateWatcher<TConfig, TState>> _watchers = new List<IUpdateWatcher<TConfig, TState>>();

			public void AddWatcher(IUpdateWatcher<TConfig, TState> watcher) {
				_watchers.Add(watcher);
			}
		}

		readonly Settings _settings;

		readonly ConcurrentDictionary<UserId, ConcurrentQueue<ICommand<TConfig, TState>>> _commands
			= new ConcurrentDictionary<UserId, ConcurrentQueue<ICommand<TConfig, TState>>>();
		readonly ConcurrentDictionary<UserId, CommandListener<TConfig, TState>> _listeners =
			new ConcurrentDictionary<UserId, CommandListener<TConfig, TState>>();

		public CommandScheduler(Settings settings) {
			_settings = settings;
		}

		public void Update() {
			foreach ( var pair in _listeners ) {
				var userId   = pair.Key;
				var listener = pair.Value;
				var task     = listener.CompletionSource.Task;
				if ( task.Status == TaskStatus.Canceled ) {
					continue;
				}
				if ( task.Status == TaskStatus.RanToCompletion ) {
					continue;
				}
				foreach ( var watcher in _settings.Watchers ) {
					watcher.OnCommandRequest(listener.Config, listener.State, userId, this);
				}
				TryApplyCommand(userId, true);
			}
		}

		public void AddCommand(UserId userId, ICommand<TConfig, TState> command, bool autoApply = false) {
			_commands.AddOrUpdate(
				userId,
				_ => CreateNewQueue(command),
				(_, commands) => UpdateQueue(commands, command));
			if ( autoApply ) {
				TryApplyCommand(userId, true);
			}
		}

		public async Task<ICommand<TConfig, TState>[]> WaitForCommands(TConfig config, TState state, UserId userId) {
			var listener = new CommandListener<TConfig, TState>(config, state);
			_listeners.AddOrUpdate(
				userId,
				_ => listener,
				(_, oldListener) => {
					oldListener.CompletionSource.TrySetCanceled();
					return listener;
				});
			foreach ( var watcher in _settings.Watchers ) {
				watcher.OnCommandRequest(config, state, userId, this);
			}
			TryApplyCommand(userId, false);
			return await listener.CompletionSource.Task;
		}

		public void CancelWaiting(UserId userId) {
			if ( _listeners.TryRemove(userId, out var listener) ) {
				listener.CompletionSource.TrySetCanceled();
			}
		}

		void TryApplyCommand(UserId userId, bool remove) {
			if ( _commands.TryGetValue(userId, out var commands) ) {
				if ( _listeners.TryRemove(userId, out var listener) ) {
					var result = new List<ICommand<TConfig, TState>>();
					while ( commands.TryDequeue(out var command) ) {
						result.Add(command);
					}
					if ( result.Count > 0 ) {
						listener.CompletionSource.TrySetResult(result.ToArray());
					}
					if ( !remove ) {
						_listeners.TryAdd(userId, listener);
					}
				}
			}
		}

		ConcurrentQueue<ICommand<TConfig, TState>> CreateNewQueue(ICommand<TConfig, TState> command) {
			return UpdateQueue(new ConcurrentQueue<ICommand<TConfig, TState>>(), command);
		}

		ConcurrentQueue<ICommand<TConfig, TState>> UpdateQueue(ConcurrentQueue<ICommand<TConfig, TState>> queue, ICommand<TConfig, TState> command) {
			queue.Enqueue(command);
			return queue;
		}
	}
}
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
		readonly ConcurrentDictionary<UserId, TaskCompletionSource<ICommand<TConfig, TState>[]>> _listeners =
			new ConcurrentDictionary<UserId, TaskCompletionSource<ICommand<TConfig, TState>[]>>();

		public CommandScheduler(Settings settings) {
			_settings = settings;
		}

		public void AddCommand(UserId userId, ICommand<TConfig, TState> command, bool autoApply = false) {
			_commands.AddOrUpdate(
				userId,
				_ => CreateNewQueue(command),
				(_, commands) => UpdateQueue(commands, command));
			if ( autoApply ) {
				TryApplyCommand(userId);
			}
		}

		public async Task<ICommand<TConfig, TState>[]> WaitForCommands(UserId userId) {
			var tcs = new TaskCompletionSource<ICommand<TConfig, TState>[]>();
			_listeners.AddOrUpdate(
				userId,
				_ => tcs,
				(_, oldTcs) => {
					oldTcs.SetCanceled();
					return tcs;
				});
			foreach ( var watcher in _settings.Watchers ) {
				watcher.OnCommandRequest(userId, this);
			}
			TryApplyCommand(userId);
			return await tcs.Task;
		}

		public void CancelWaiting(UserId userId) {
			if ( _listeners.TryRemove(userId, out var tcs) ) {
				tcs.SetCanceled();
			}
		}

		void TryApplyCommand(UserId userId) {
			if ( _commands.TryGetValue(userId, out var commands) ) {
				if ( _listeners.TryRemove(userId, out var tcs) ) {
					var result = new List<ICommand<TConfig, TState>>();
					while ( commands.TryDequeue(out var command) ) {
						result.Add(command);
					}
					if ( result.Count > 0 ) {
						tcs.SetResult(result.ToArray());
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
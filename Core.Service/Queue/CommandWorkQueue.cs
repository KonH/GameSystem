using System;
using System.Collections.Concurrent;
using Core.Common.Config;
using Core.Common.State;
using Core.Service.Model;

namespace Core.Service.Queue {
	public sealed class CommandWorkQueue<TConfig, TState> where TConfig : IConfig where TState : IState {
		readonly ConcurrentDictionary<UserId, CommandWorkItem<TConfig, TState>> _items =
			new ConcurrentDictionary<UserId, CommandWorkItem<TConfig, TState>>();

		public bool Enqueue(UserId userId, CommandWorkItem<TConfig, TState> item) {
			return _items.TryAdd(userId, item);
		}

		public void Dequeue(UserId userId) {
			_items.TryRemove(userId, out _);
		}

		public void ForEachPending(Action<UserId, CommandWorkItem<TConfig, TState>> callback) {
			foreach ( var pair in _items ) {
				var userId = pair.Key;
				var item   = pair.Value;
				if ( item.IsCompleted ) {
					continue;
				}
				callback(userId, item);
			}
		}
	}
}
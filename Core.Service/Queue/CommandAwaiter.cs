using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Common.Config;
using Core.Common.State;
using Core.Service.Model;

namespace Core.Service.Queue {
	public sealed class CommandAwaiter<TConfig, TState> where TConfig : IConfig where TState : IState {
		readonly CommandWorkQueue<TConfig, TState> _queue;

		public event Action OnWait;

		public CommandAwaiter(CommandWorkQueue<TConfig, TState> queue) {
			_queue = queue;
		}

		public Task<CommandQueueResult<TConfig, TState>> WaitForCommands(UserId userId, TConfig config, TState state, CancellationToken cancellationToken) {
			var item = new CommandWorkItem<TConfig, TState>(config, state);
			if ( !_queue.Enqueue(userId, item) ) {
				return Task.FromCanceled<CommandQueueResult<TConfig, TState>>(cancellationToken);
			}
			OnWait?.Invoke();
			return item.Task;
		}

		public void CancelWaiting(UserId userId) {
			_queue.Dequeue(userId);
		}
	}
}
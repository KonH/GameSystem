using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Common.Threading {
	public sealed class DefaultTaskRunner : ITaskRunner {
		public void Run(Func<Task> action, CancellationToken cancellationToken) {
			Task.Run(action, cancellationToken);
		}

		public Task Delay(TimeSpan delay) {
			return Task.Delay(delay);
		}
	}
}
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Common.Threading {
	public interface ITaskRunner {
		void Run(Func<CancellationToken, Task> action, CancellationToken cancellationToken);
		Task Delay(TimeSpan delay);
	}
}
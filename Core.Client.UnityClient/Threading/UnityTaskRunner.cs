using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Common.Threading;
using UnityEngine;

namespace Core.Client.UnityClient.Threading {
	public sealed class UnityTaskRunner : ITaskRunner {
		public void Run(Func<CancellationToken, Task> action, CancellationToken cancellationToken) {
			UnityThread.Post(() => action(cancellationToken));
		}

		public async Task Delay(TimeSpan delay) {
			await new WaitForSeconds((float)delay.TotalSeconds);
		}
	}
}
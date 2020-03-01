using System;
using System.Runtime.CompilerServices;

namespace Core.Client.UnityClient.Threading {
	public sealed class BackgroundThreadAwaiter : INotifyCompletion {
		readonly CoroutineAwaiter _coroutineAwaiter;

		ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _configuredTaskAwaiter;

		public bool IsCompleted => (_coroutineAwaiter != null)
			? _coroutineAwaiter.IsCompleted
			: _configuredTaskAwaiter.IsCompleted;

		public BackgroundThreadAwaiter(CoroutineAwaiter coroutineAwaiter) {
			_coroutineAwaiter = coroutineAwaiter;
		}

		public BackgroundThreadAwaiter(ConfiguredTaskAwaitable.ConfiguredTaskAwaiter configuredTaskAwaiter) {
			_configuredTaskAwaiter = configuredTaskAwaiter;
		}

		public void GetResult() {
			if ( _coroutineAwaiter != null ) {
				_coroutineAwaiter.GetResult();
			} else {
				_configuredTaskAwaiter.GetResult();
			}
		}

		void INotifyCompletion.OnCompleted(Action continuation) {
			if ( _coroutineAwaiter != null ) {
				((INotifyCompletion) _coroutineAwaiter).OnCompleted(continuation);
			} else {
				_configuredTaskAwaiter.OnCompleted(continuation);
			}
		}
	}
}
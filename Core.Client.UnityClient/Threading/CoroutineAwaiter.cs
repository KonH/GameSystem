using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Core.Client.UnityClient.Threading {
	public sealed class CoroutineAwaiter : INotifyCompletion {
		bool      _isDone;
		Exception _exception;
		Action    _continuation;

		public bool IsCompleted {
			get { return _isDone; }
		}

		public void GetResult() {
			if ( _exception != null ) {
				ExceptionDispatchInfo.Capture(_exception).Throw();
			}
		}

		public void Complete(Exception e) {
			_isDone    = true;
			_exception = e;

			if ( _continuation != null ) {
				UnityThread.Run(_continuation);
			}
		}

		void INotifyCompletion.OnCompleted(Action continuation) {
			_continuation = continuation;
		}
	}
}
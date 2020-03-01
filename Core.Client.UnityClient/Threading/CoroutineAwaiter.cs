using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Core.Client.UnityClient.Threading {
	public sealed class CoroutineAwaiter : INotifyCompletion {
		bool      _isDone;
		Exception _exception;
		Action    _continuation;

		public bool IsCompleted => _isDone;

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

	public class CoroutineAwaiter<T> : INotifyCompletion {
		bool      _isDone;
		Exception _exception;
		Action    _continuation;
		T         _result;

		public bool IsCompleted => _isDone;

		public T GetResult() {
			if ( _exception != null ) {
				ExceptionDispatchInfo.Capture(_exception).Throw();
			}
			return _result;
		}

		public void Complete(T result, Exception e) {
			_isDone    = true;
			_exception = e;
			_result    = result;

			if ( _continuation != null ) {
				UnityThread.Run(_continuation);
			}
		}

		void INotifyCompletion.OnCompleted(Action continuation) {
			_continuation = continuation;
		}
	}
}
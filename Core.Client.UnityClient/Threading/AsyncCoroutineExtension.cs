using UnityEngine;

namespace Core.Client.UnityClient.Threading {
	public static class AsyncCoroutineExtension {
		public static CoroutineAwaiter GetAwaiter(this WaitForUpdate instruction) {
			return GetAwaiterReturnVoid(instruction);
		}

		public static CoroutineAwaiter<AsyncOperation> GetAwaiter(this AsyncOperation instruction) {
			return GetAwaiterReturnSelf(instruction);
		}

		static CoroutineAwaiter GetAwaiterReturnVoid(object instruction) {
			var awaiter = new CoroutineAwaiter();
			UnityThread.Run(() => UnityThreadRunner.Instance.StartCoroutine(
				CoroutineWrapper.Void(awaiter, instruction)));
			return awaiter;
		}

		static CoroutineAwaiter<T> GetAwaiterReturnSelf<T>(T instruction) {
			var awaiter = new CoroutineAwaiter<T>();
			UnityThread.Run(() => UnityThreadRunner.Instance.StartCoroutine(
				CoroutineWrapper.Self(awaiter, instruction)));
			return awaiter;
		}
	}
}
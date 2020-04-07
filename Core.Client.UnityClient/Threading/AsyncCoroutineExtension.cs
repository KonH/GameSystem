using System.Threading.Tasks;
using UnityEngine;

namespace Core.Client.UnityClient.Threading {
	public static class AsyncCoroutineExtension {
		public static BackgroundThreadAwaiter GetAwaiter(this WaitForBackgroundThread _) {
			if ( Application.platform == RuntimePlatform.WebGLPlayer ) {
				return new BackgroundThreadAwaiter(Async.WaitForUpdate.GetAwaiter());
			}
			var awaiter = Task.Run(() => {}).ConfigureAwait(false).GetAwaiter();
			return new BackgroundThreadAwaiter(awaiter);
		}

		public static CoroutineAwaiter GetAwaiter(this WaitForUpdate instruction) {
			return GetAwaiterReturnVoid(instruction);
		}

		public static CoroutineAwaiter GetAwaiter(this WaitForSeconds instruction) {
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
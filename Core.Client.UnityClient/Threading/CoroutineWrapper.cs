using System.Collections;

namespace Core.Client.UnityClient.Threading {
	static class CoroutineWrapper {
		public static IEnumerator Void(CoroutineAwaiter awaiter, object instruction) {
			yield return instruction;
			awaiter.Complete(null);
		}

		public static IEnumerator Self<T>(CoroutineAwaiter<T> awaiter, T instruction) {
			yield return instruction;
			awaiter.Complete(instruction, null);
		}
	}
}
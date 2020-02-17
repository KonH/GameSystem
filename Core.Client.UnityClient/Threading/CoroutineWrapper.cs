using System.Collections;

namespace Core.Client.UnityClient.Threading {
	static class CoroutineWrapper {
		public static IEnumerator Void(CoroutineAwaiter awaiter, object instruction) {
			yield return instruction;
			awaiter.Complete(null);
		}
	}
}
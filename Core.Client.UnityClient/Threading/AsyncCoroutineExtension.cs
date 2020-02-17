namespace Core.Client.UnityClient.Threading {
	public static class AsyncCoroutineExtension {
		public static CoroutineAwaiter GetAwaiter(this WaitForUpdate instruction) {
			return GetAwaiterReturnVoid(instruction);
		}

		static CoroutineAwaiter GetAwaiterReturnVoid(object instruction) {
			var awaiter = new CoroutineAwaiter();
			UnityThread.Run(() => UnityThreadRunner.Instance.StartCoroutine(
				CoroutineWrapper.Void(awaiter, instruction)));
			return awaiter;
		}
	}
}
namespace Core.Client.UnityClient.Threading {
	public static class Async {
		public static readonly WaitForUpdate           WaitForUpdate           = new WaitForUpdate();
		public static readonly WaitForBackgroundThread WaitForBackgroundThread = new WaitForBackgroundThread();
	}
}
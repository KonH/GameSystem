using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Core.Client.UnityClient.Threading {
	public sealed class WaitForBackgroundThread {
		public ConfiguredTaskAwaitable.ConfiguredTaskAwaiter GetAwaiter() {
			return Task.Run(() => { }).ConfigureAwait(false).GetAwaiter();
		}
	}
}
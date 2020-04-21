using System.Threading.Tasks;
using Core.Client.Web;
using Idler.Common.State;

namespace Idler.UnityClient.Repository {
	public sealed class WebSharedStateProvider : ISharedStateProvider {
		readonly WebClientHandler _handler;

		public WebSharedStateProvider(WebClientHandler handler) {
			_handler = handler;
		}

		public async Task<int> GetResourceCount() {
			var result = await _handler.Get("sharedState", e => new SharedState());
			return result.Resources;
		}
	}
}
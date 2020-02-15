using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Client.Web;

namespace Core.Client.WebClient {
	public sealed class StandardWebRequestHandler : IWebRequestHandler {
		readonly System.Net.WebClient _webClient;

		public StandardWebRequestHandler(string baseAddress) {
			_webClient = new System.Net.WebClient {
				BaseAddress = baseAddress
			};
		}

		public async Task<ServiceResponse> Post(string url, string body) {
			_webClient.Headers.Add("Content-Type", "application/json");
			try {
				var result = await _webClient.UploadStringTaskAsync(url, HttpMethod.Post.ToString(), body);
				return new ServiceResponse.Ok<string>(result);
			} catch ( WebException e ) {
				return new ServiceResponse.Error(e.ToString());
			}
		}
	}
}
using System.Net;
using System.Net.Http;

namespace Core.Client.WebClient {
	// TODO: async
	public sealed class StandardWebRequestHandler : IWebRequestHandler {
		readonly System.Net.WebClient _webClient;

		public StandardWebRequestHandler(string baseAddress) {
			_webClient = new System.Net.WebClient {
				BaseAddress = baseAddress
			};
		}

		public ServiceResponse Post(string url, string body) {
			_webClient.Headers.Add("Content-Type", "application/json");
			try {
				var result = _webClient.UploadString(url, HttpMethod.Post.ToString(), body);
				return new ServiceResponse.Ok<string>(result);
			} catch ( WebException e ) {
				return new ServiceResponse.Error(e.ToString());
			}
		}
	}
}
using System.Net.Http;

namespace Core.Client.WebClient {
	// TODO: async
	// TODO: handle errors
	public sealed class StandardWebRequestHandler : IWebRequestHandler {
		readonly System.Net.WebClient _webClient;

		public StandardWebRequestHandler(string baseAddress) {
			_webClient = new System.Net.WebClient {
				BaseAddress = baseAddress
			};
		}

		public string Post(string url, string body) {
			_webClient.Headers.Add("Content-Type", "application/json");
			return _webClient.UploadString(url, HttpMethod.Post.ToString(), body);
		}
	}
}
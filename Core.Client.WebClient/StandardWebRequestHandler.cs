using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Client.Web;

namespace Core.Client.WebClient {
	public sealed class StandardWebRequestHandler : IWebRequestHandler {
		readonly string _baseAddress;

		public StandardWebRequestHandler(string baseAddress) {
			_baseAddress = baseAddress;
		}

		public async Task<ServiceResponse> Get(string url) {
			var webClient = new System.Net.WebClient {
				BaseAddress = _baseAddress
			};
			webClient.Headers.Add("Content-Type", "application/json");
			try {
				var result = await webClient.DownloadStringTaskAsync(url);
				return new ServiceResponse.Ok<string>(result);
			} catch ( WebException e ) {
				return new ServiceResponse.Error(e.ToString());
			}
		}

		public async Task<ServiceResponse> Post(string url, string body) {
			var webClient = new System.Net.WebClient {
				BaseAddress = _baseAddress
			};
			webClient.Headers.Add("Content-Type", "application/json");
			try {
				var result = await webClient.UploadStringTaskAsync(url, HttpMethod.Post.ToString(), body);
				return new ServiceResponse.Ok<string>(result);
			} catch ( WebException e ) {
				return new ServiceResponse.Error(e.ToString());
			}
		}
	}
}
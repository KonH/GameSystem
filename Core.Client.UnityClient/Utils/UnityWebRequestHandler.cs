using System.Text;
using System.Threading.Tasks;
using Core.Client.UnityClient.Threading;
using Core.Client.Web;
using UnityEngine.Networking;

namespace Core.Client.UnityClient.Utils {
	public sealed class UnityWebRequestHandler : IWebRequestHandler {
		public sealed class Settings {
			public readonly string BaseUrl;

			public Settings(string baseUrl) {
				BaseUrl = baseUrl;
			}
		}

		readonly Settings _settings;

		public UnityWebRequestHandler(Settings settings) {
			_settings = settings;
		}

		public async Task<ServiceResponse> Get(string url) {
			await Async.WaitForUpdate;
			var req = new UnityWebRequest(_settings.BaseUrl + url, UnityWebRequest.kHttpVerbGET) {
				downloadHandler = new DownloadHandlerBuffer()
			};
			req.SetRequestHeader("Content-Type", "application/json");
			await req.SendWebRequest();
			if ( req.isNetworkError || req.isHttpError ) {
				return new ServiceResponse.Error($"{req.error}: {req.downloadHandler.text}");
			}
			return new ServiceResponse.Ok<string>(req.downloadHandler.text);
		}

		public async Task<ServiceResponse> Post(string url, string body) {
			await Async.WaitForUpdate;
			var bodyBytes = Encoding.UTF8.GetBytes(body);
			var req = new UnityWebRequest(_settings.BaseUrl + url, UnityWebRequest.kHttpVerbPOST) {
				uploadHandler   = new UploadHandlerRaw(bodyBytes),
				downloadHandler = new DownloadHandlerBuffer()
			};
			req.SetRequestHeader("Content-Type", "application/json");
			await req.SendWebRequest();
			if ( req.isNetworkError || req.isHttpError ) {
				return new ServiceResponse.Error($"{req.error}: {req.downloadHandler.text}");
			}
			return new ServiceResponse.Ok<string>(req.downloadHandler.text);
		}
	}
}
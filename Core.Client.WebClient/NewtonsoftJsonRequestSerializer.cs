using Newtonsoft.Json;

namespace Core.Client.WebClient {
	public sealed class NewtonsoftJsonRequestSerializer : IRequestSerializer {
		readonly JsonSerializerSettings _settings = new JsonSerializerSettings {
			TypeNameHandling = TypeNameHandling.Auto
		};

		public string Serialize<T>(T instance) {
			return JsonConvert.SerializeObject(instance, _settings);
		}

		public T Deserialize<T>(string content) {
			return JsonConvert.DeserializeObject<T>(content, _settings);
		}
	}
}
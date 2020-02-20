using Newtonsoft.Json;

namespace Core.Client.Utils {
	public sealed class JsonObjectPresenter {
		readonly JsonSerializerSettings _serializerSettings;

		public JsonObjectPresenter(JsonSerializerSettings serializerSettings) {
			_serializerSettings = serializerSettings;
		}

		public string Format(object obj) {
			return JsonConvert.SerializeObject(obj, _serializerSettings);
		}
	}
}
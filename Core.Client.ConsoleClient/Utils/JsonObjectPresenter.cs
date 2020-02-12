using System.Text.Json;

namespace Core.Client.ConsoleClient.Utils {
	sealed class JsonObjectPresenter {
		readonly JsonSerializerOptions _serializerOptions;

		public JsonObjectPresenter(JsonSerializerOptions serializerOptions) {
			_serializerOptions = serializerOptions;
		}

		public string Format(object obj) {
			return JsonSerializer.Serialize(obj, _serializerOptions);
		}
	}
}
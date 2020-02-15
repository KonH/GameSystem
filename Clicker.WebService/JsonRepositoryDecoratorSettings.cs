using System.Text.Json;
using Core.Service.Repository;

namespace Clicker.WebService {
	public static class JsonRepositoryDecoratorSettings {
		public static InMemoryRepositoryDecorator<T, string>.Settings Create<T>() {
			return new InMemoryRepositoryDecorator<T, string>.Settings(
				state => JsonSerializer.Serialize(state),
				s => JsonSerializer.Deserialize<T>(s));
		}
	}
}
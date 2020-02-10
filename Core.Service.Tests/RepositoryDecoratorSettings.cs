using System.Text.Json;
using Core.Service.Repository;

namespace Core.Service.Tests {
	public sealed class RepositoryDecoratorSettings {
		public static InMemoryRepositoryDecorator<T, string>.Settings Create<T>() {
			return new InMemoryRepositoryDecorator<T, string>.Settings(
				state => JsonSerializer.Serialize(state),
				s => JsonSerializer.Deserialize<T>(s));
		}
	}
}
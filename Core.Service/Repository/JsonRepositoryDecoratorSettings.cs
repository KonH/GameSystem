using Newtonsoft.Json;

namespace Core.Service.Repository {
	public static class JsonRepositoryDecoratorSettings {
		public static InMemoryRepositoryDecorator<T, string>.Settings Create<T>() {
			return new InMemoryRepositoryDecorator<T, string>.Settings(
				state => JsonConvert.SerializeObject(state),
				JsonConvert.DeserializeObject<T>);
		}
	}
}
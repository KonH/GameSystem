using Core.Common.Config;

namespace Core.Service.Repository.Config {
	public sealed class InMemoryConfigRepository<TConfig> :
		InMemoryRepositoryDecorator<TConfig, string>, IConfigRepository<TConfig> where TConfig : IConfig {
		public InMemoryConfigRepository(Settings settings) : base(settings) { }
	}
}
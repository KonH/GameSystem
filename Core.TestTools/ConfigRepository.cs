using Core.Common.Config;
using Core.Service.Extension;
using Core.Service.Repository;
using Core.Service.Repository.Config;

namespace Core.TestTools {
	public static class ConfigRepository<TConfig> where TConfig : IConfig {
		public static IConfigRepository<TConfig> Create(params TConfig[] configs) {
			var settings   = JsonRepositoryDecoratorSettings.Create<TConfig>();
			var repository = new InMemoryConfigRepository<TConfig>(settings);
			foreach ( var config in configs ) {
				repository.Add(config);
			}
			return repository;
		}
	}
}
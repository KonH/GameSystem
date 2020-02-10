using Core.Common.Config;
using Core.Service.Extension;
using Core.Service.Repository.Config;

namespace Core.Service.Tests {
	public static class ConfigRepository<TConfig> where TConfig : IConfig, new() {
		public static IConfigRepository<TConfig> Create() {
			var settings = RepositoryDecoratorSettings.Create<TConfig>();
			var repository = new InMemoryConfigRepository<TConfig>(settings);
			repository.Add(new TConfig());
			return repository;
		}
	}
}
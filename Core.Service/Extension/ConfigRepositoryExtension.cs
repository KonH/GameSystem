using Core.Common.Config;
using Core.Service.Repository.Config;

namespace Core.Service.Extension {
	public static class ConfigRepositoryExtension {
		public static void Add<TConfig>(this IConfigRepository<TConfig> repository, TConfig config)
			where TConfig : IConfig {
			repository.Add(config.Version.Value, config);
		}

		public static TConfig Get<TConfig>(this IConfigRepository<TConfig> repository, ConfigVersion configVersion)
			where TConfig : IConfig {
			return repository.Get(configVersion.Value);
		}
	}
}
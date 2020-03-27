using System.Threading.Tasks;
using Core.Common.Config;
using Core.Service.Repository.Config;

namespace Core.Service.Extension {
	public static class ConfigRepositoryExtension {
		public static Task Add<TConfig>(this IConfigRepository<TConfig> repository, TConfig config)
			where TConfig : IConfig {
			return repository.Add(config.Version.Value, config);
		}

		public static Task<TConfig> Get<TConfig>(this IConfigRepository<TConfig> repository, ConfigVersion configVersion)
			where TConfig : IConfig {
			return repository.Get(configVersion.Value);
		}
	}
}
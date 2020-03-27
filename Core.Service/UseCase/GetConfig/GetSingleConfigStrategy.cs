using System;
using System.Threading.Tasks;
using Core.Common.Config;
using Core.Service.Extension;
using Core.Service.Model;
using Core.Service.Repository.Config;

namespace Core.Service.UseCase.GetConfig {
	public sealed class GetSingleConfigStrategy<TConfig> : IGetConfigStrategy<TConfig> where TConfig : IConfig {
		public sealed class Settings {
			public readonly ConfigVersion Version;

			public Settings(ConfigVersion version) {
				Version = version;
			}
		}

		readonly Settings                   _settings;
		readonly IConfigRepository<TConfig> _configRepository;

		public GetSingleConfigStrategy(Settings settings, IConfigRepository<TConfig> configRepository) {
			if ( settings.Version?.Value == null ) {
				throw new ArgumentNullException(nameof(settings));
			}

			_settings         = settings;
			_configRepository = configRepository;
		}

		public Task<TConfig> GetUserConfig(UserId userId) {
			return _configRepository.Get(_settings.Version);
		}
	}
}
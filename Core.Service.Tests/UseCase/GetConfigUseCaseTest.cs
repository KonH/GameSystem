using System;
using Core.Common.Config;
using Core.Service.Model;
using Core.Service.UseCase.GetConfig;
using NUnit.Framework;

namespace Core.Service.Tests.UseCase {
	public sealed class GetConfigUseCaseTest {
		sealed class Config : IConfig {
			public ConfigVersion Version { get; set; } = new ConfigVersion();
		}

		[Test]
		public void IsSingleConfigStrategyRequireNotNullVersion() {
			var settings = new GetSingleConfigStrategy<Config>.Settings(null);
			var repository = ConfigRepository<Config>.Create();

			Assert.Throws<ArgumentNullException>(() => {
				var _ = new GetSingleConfigStrategy<Config>(settings, repository);
			});
		}

		[Test]
		public void IsSingleConfigStrategyRequireValidVersion() {
			var settings   = new GetSingleConfigStrategy<Config>.Settings(new ConfigVersion(null));
			var repository = ConfigRepository<Config>.Create();

			Assert.Throws<ArgumentNullException>(() => {
				var _ = new GetSingleConfigStrategy<Config>(settings, repository);
			});
		}

		[Test]
		public void IsConfigFound() {
			var useCase = GetUseCase(
				new ConfigVersion("TestVersion"),
				new Config { Version = new ConfigVersion("TestVersion") });
			var req = GetRequest();

			var resp = useCase.Handle(req);

			Assert.IsInstanceOf<GetConfigResponse<Config>.Found>(resp);
			Assert.NotNull(((GetConfigResponse<Config>.Found) resp).Config);
		}

		[Test]
		public void IsConfigNotFound() {
			var useCase = GetUseCase(new ConfigVersion("TestVersion"));
			var req     = GetRequest();

			var resp = useCase.Handle(req);

			Assert.IsInstanceOf<GetConfigResponse<Config>.NotFound>(resp);
		}

		GetConfigUseCase<Config> GetUseCase(ConfigVersion configVersion, params Config[] configs) {
			var repository = ConfigRepository<Config>.Create(configs);
			var settings   = new GetSingleConfigStrategy<Config>.Settings(configVersion);
			var strategy   = new GetSingleConfigStrategy<Config>(settings, repository);
			return new GetConfigUseCase<Config>(strategy);
		}

		GetConfigRequest GetRequest() {
			return new GetConfigRequest {
				UserId = new UserId("UserId")
			};
		}
	}
}
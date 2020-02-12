using Core.Common.Config;

namespace Core.Service.UseCase.GetConfig {
	public abstract class GetConfigResponse<TConfig> where TConfig : IConfig {
		public sealed class Found : GetConfigResponse<TConfig> {
			public TConfig Config { get; set; }

			public Found() { }

			public Found(TConfig config) {
				Config = config;
			}
		}

		public sealed class NotFound : GetConfigResponse<TConfig> { }

		public sealed class BadRequest : GetConfigResponse<TConfig> {
			public string Description { get; set; }

			public BadRequest() { }

			public BadRequest(string description) {
				Description = description;
			}
		}

		GetConfigResponse() { }
	}
}
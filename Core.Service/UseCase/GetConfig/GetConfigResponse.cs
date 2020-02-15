using Core.Common.Config;

namespace Core.Service.UseCase.GetConfig {
	public abstract class GetConfigResponse {
		public sealed class Found<TConfig> : GetConfigResponse where TConfig : IConfig {
			public TConfig Config { get; set; }

			public Found() { }

			public Found(TConfig config) {
				Config = config;
			}
		}

		public sealed class NotFound : GetConfigResponse { }

		public sealed class BadRequest : GetConfigResponse {
			public string Description { get; set; }

			public BadRequest() { }

			public BadRequest(string description) {
				Description = description;
			}
		}

		GetConfigResponse() { }
	}
}
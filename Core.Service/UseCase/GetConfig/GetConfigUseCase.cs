using Core.Common.Config;

namespace Core.Service.UseCase.GetConfig {
	public sealed class GetConfigUseCase<TConfig> : IUseCase<GetConfigRequest, GetConfigResponse<TConfig>>
		where TConfig : IConfig {
		readonly IGetConfigStrategy<TConfig> _strategy;

		public GetConfigUseCase(IGetConfigStrategy<TConfig> strategy) {
			_strategy = strategy;
		}

		public GetConfigResponse<TConfig> Handle(GetConfigRequest request) {
			var validateError = Validate(request);
			if ( validateError != null ) {
				return validateError;
			}
			var config = _strategy.GetUserConfig(request.UserId);
			return (config != null) ? Found(config) : NotFound();
		}

		GetConfigResponse<TConfig> Validate(GetConfigRequest request) {
			if ( request == null ) {
				return BadRequest("null request");
			}
			if ( request.UserId == null ) {
				return BadRequest("null user id");
			}
			return null;
		}

		static GetConfigResponse<TConfig>.BadRequest BadRequest(string description) {
			return new GetConfigResponse<TConfig>.BadRequest(description);
		}

		static GetConfigResponse<TConfig>.NotFound NotFound() {
			return new GetConfigResponse<TConfig>.NotFound();
		}

		static GetConfigResponse<TConfig> Found(TConfig config) {
			return new GetConfigResponse<TConfig>.Found(config);
		}
	}
}
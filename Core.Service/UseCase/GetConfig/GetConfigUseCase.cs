using System.Threading.Tasks;
using Core.Common.Config;

namespace Core.Service.UseCase.GetConfig {
	public sealed class GetConfigUseCase<TConfig> : IUseCase<GetConfigRequest, GetConfigResponse>
		where TConfig : IConfig {
		readonly IGetConfigStrategy<TConfig> _strategy;

		public GetConfigUseCase(IGetConfigStrategy<TConfig> strategy) {
			_strategy = strategy;
		}

		public async Task<GetConfigResponse> Handle(GetConfigRequest request) {
			var validateError = Validate(request);
			if ( validateError != null ) {
				return validateError;
			}
			var config = await _strategy.GetUserConfig(request.UserId);
			return (config != null) ? Found(config) : NotFound();
		}

		GetConfigResponse Validate(GetConfigRequest request) {
			if ( request == null ) {
				return BadRequest("null request");
			}
			if ( request.UserId == null ) {
				return BadRequest("null user id");
			}
			return null;
		}

		static GetConfigResponse.BadRequest BadRequest(string description) {
			return new GetConfigResponse.BadRequest(description);
		}

		static GetConfigResponse.NotFound NotFound() {
			return new GetConfigResponse.NotFound();
		}

		static GetConfigResponse Found(TConfig config) {
			return new GetConfigResponse.Found<TConfig>(config);
		}
	}
}
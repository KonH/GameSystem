using Core.Common.Config;
using Core.Common.State;
using Core.Common.Utils;
using Core.Service.UseCase.GetConfig;
using Core.Service.UseCase.GetState;
using Core.Service.UseCase.UpdateState;

namespace Core.Client {
	public sealed class WebClientHandler {
		readonly IRequestSerializer _serializer;
		readonly IWebRequestHandler _handler;

		public WebClientHandler(IRequestSerializer serializer, IWebRequestHandler handler) {
			_serializer = serializer;
			_handler    = handler;
		}

		public GetConfigResponse<TConfig> GetConfig<TConfig>(GetConfigRequest request)
			where TConfig : IConfig =>
			Post<GetConfigRequest, GetConfigResponse<TConfig>>("config/get", request);

		public GetStateResponse<TState> GetState<TState>(GetStateRequest request)
			where TState : IState =>
			Post<GetStateRequest, GetStateResponse<TState>>("state/get", request);

		public UpdateStateResponse<TConfig, TState> UpdateState<TConfig, TState>(
			UpdateStateRequest<TConfig, TState> request)
			where TConfig : IConfig where TState : IState =>
			Post<UpdateStateRequest<TConfig, TState>, UpdateStateResponse<TConfig, TState>>("state/update", request);

		TResponse Post<TRequest, TResponse>(string url, TRequest request) {
			var body     = _serializer.Serialize(request);
			var result   = _handler.Post(url, body);
			var response = _serializer.Deserialize<WebResponse<TResponse>>(result);
			return response.Data;
		}
	}
}
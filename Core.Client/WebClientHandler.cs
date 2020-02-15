using System;
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
			Post<GetConfigRequest, GetConfigResponse<TConfig>>(
				"config/get", request, e => new GetConfigResponse<TConfig>.BadRequest(e.Description));

		public GetStateResponse<TState> GetState<TState>(GetStateRequest request)
			where TState : IState =>
			Post<GetStateRequest, GetStateResponse<TState>>(
				"state/get", request, e => new GetStateResponse<TState>.BadRequest(e.Description));

		public UpdateStateResponse<TConfig, TState> UpdateState<TConfig, TState>(
			UpdateStateRequest<TConfig, TState> request)
			where TConfig : IConfig where TState : IState =>
			Post<UpdateStateRequest<TConfig, TState>, UpdateStateResponse<TConfig, TState>>(
				"state/update", request, e => new UpdateStateResponse<TConfig, TState>.BadRequest(e.Description));

		TResponse Post<TRequest, TResponse>(
			string url, TRequest request, Func<ServiceResponse<string>.Error, TResponse> errorHandler) {
			var body     = _serializer.Serialize(request);
			var result   = _handler.Post(url, body);
			switch ( result ) {
				case ServiceResponse<string>.Ok ok: {
					var response = _serializer.Deserialize<WebResponse<TResponse>>(ok.Result);
					return response.Data;
				}

				case ServiceResponse<string>.Error error:
					return errorHandler(error);

				default:
					throw new ArgumentOutOfRangeException(nameof(result), result, null);
			}
		}
	}
}
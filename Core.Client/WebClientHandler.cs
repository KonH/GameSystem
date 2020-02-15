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

		public GetConfigResponse GetConfig(GetConfigRequest request) =>
			Post<GetConfigRequest, GetConfigResponse>(
				"config/get", request, e => new GetConfigResponse.BadRequest(e.Description));

		public GetStateResponse GetState<TState>(GetStateRequest request)
			where TState : IState =>
			Post<GetStateRequest, GetStateResponse>(
				"state/get", request, e => new GetStateResponse.BadRequest(e.Description));

		public UpdateStateResponse UpdateState<TConfig, TState>(
			UpdateStateRequest<TConfig, TState> request)
			where TConfig : IConfig where TState : IState =>
			Post<UpdateStateRequest<TConfig, TState>, UpdateStateResponse>(
				"state/update", request, e => new UpdateStateResponse.BadRequest(e.Description));

		TResponse Post<TRequest, TResponse>(
			string url, TRequest request, Func<ServiceResponse.Error, TResponse> errorHandler) {
			var body     = _serializer.Serialize(request);
			var result   = _handler.Post(url, body);
			switch ( result ) {
				case ServiceResponse.Ok<string> ok: {
					var response = _serializer.Deserialize<WebResponse<TResponse>>(ok.Result);
					return response.Data;
				}

				case ServiceResponse.Error error:
					return errorHandler(error);

				default:
					throw new ArgumentOutOfRangeException(nameof(result), result, null);
			}
		}
	}
}
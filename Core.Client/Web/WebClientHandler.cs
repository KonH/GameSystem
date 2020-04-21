using System;
using System.Threading.Tasks;
using Core.Common.Config;
using Core.Common.State;
using Core.Common.Utils;
using Core.Service.UseCase.GetConfig;
using Core.Service.UseCase.GetState;
using Core.Service.UseCase.SendCommand;
using Core.Service.UseCase.WaitCommand;

namespace Core.Client.Web {
	public sealed class WebClientHandler {
		readonly IRequestSerializer _serializer;
		readonly IWebRequestHandler _handler;

		public WebClientHandler(IRequestSerializer serializer, IWebRequestHandler handler) {
			_serializer = serializer;
			_handler    = handler;
		}

		public Task<GetConfigResponse> GetConfig(GetConfigRequest request) =>
			Post<GetConfigRequest, GetConfigResponse>(
				"config/get", request, e => new GetConfigResponse.BadRequest(e.Description));

		public Task<GetStateResponse> GetState(GetStateRequest request) =>
			Post<GetStateRequest, GetStateResponse>(
				"state/get", request, e => new GetStateResponse.BadRequest(e.Description));

		public Task<SendCommandResponse> SendCommand<TConfig, TState>(
			SendCommandRequest<TConfig, TState> request)
			where TConfig : IConfig where TState : IState =>
			Post<SendCommandRequest<TConfig, TState>, SendCommandResponse>(
				"state/send", request, e => new SendCommandResponse.BadRequest(e.Description));

		public Task<WaitCommandResponse> WaitCommand(WaitCommandRequest request) =>
			Post<WaitCommandRequest, WaitCommandResponse>(
				"state/wait", request, e => new WaitCommandResponse.BadRequest(e.Description));

		public async Task<TResponse> Get<TResponse>(
			string url, Func<ServiceResponse.Error, TResponse> errorHandler) {
			var result = await _handler.Get(url);
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

		public async Task<TResponse> Post<TRequest, TResponse>(
			string url, TRequest request, Func<ServiceResponse.Error, TResponse> errorHandler) {
			var body     = _serializer.Serialize(request);
			var result   = await _handler.Post(url, body);
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
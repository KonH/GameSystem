using System.Threading.Tasks;
using Clicker.Common.Config;
using Clicker.Common.State;
using Core.Common.Utils;
using Core.Service.UseCase.GetState;
using Core.Service.UseCase.SendCommand;
using Core.Service.UseCase.WaitCommand;
using Core.Service.WebService.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Clicker.WebService.Controllers {
	[ApiController]
	[Route("[controller]")]
	public sealed class StateController : ControllerBase {
		readonly ILogger<StateController>                  _logger;
		readonly GetStateUseCase<GameState>                _getStateUseCase;
		readonly SendCommandUseCase<GameConfig, GameState> _sendCommandUseCase;
		readonly WaitCommandUseCase<GameConfig, GameState> _waitCommandUseCase;

		public StateController(
			ILoggerFactory loggerFactory,
			GetStateUseCase<GameState> getStateUseCase,
			SendCommandUseCase<GameConfig, GameState> sendCommandUseCase,
			WaitCommandUseCase<GameConfig, GameState> waitCommandUseCase) {
			_logger             = loggerFactory.Create<StateController>();
			_getStateUseCase    = getStateUseCase;
			_sendCommandUseCase = sendCommandUseCase;
			_waitCommandUseCase = waitCommandUseCase;
		}

		[HttpPost("get")]
		public async Task<WebResponse> Get([FromBody] GetStateRequest request) {
			_logger.LogTrace($"Get state for user: '{request.UserId?.Value}'.");
			var response = await _getStateUseCase.Handle(request);
			_logger.LogTrace($"Response is '{response.GetType().Name}'.");
			return new WebResponse(response);
		}

		[HttpPost("send")]
		public async Task<WebResponse> Send([FromBody] SendCommandRequest<GameConfig, GameState> request) {
			_logger.LogTrace($"Send command for user: '{request.UserId?.Value}'.");
			var response = await _sendCommandUseCase.Handle(request);
			_logger.LogTrace($"Response is '{response.GetType().Name}'.");
			return new WebResponse(response);
		}

		[HttpPost("wait")]
		public async Task<WebResponse> Wait([FromBody] WaitCommandRequest request) {
			_logger.LogTrace($"Wait command for user: '{request.UserId?.Value}'.");
			var response = await _waitCommandUseCase.Handle(request);
			_logger.LogTrace($"Response is '{response.GetType().Name}'.");
			return new WebResponse(response);
		}
	}
}
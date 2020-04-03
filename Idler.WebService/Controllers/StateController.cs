using System.Threading.Tasks;
using Idler.Common.Config;
using Idler.Common.State;
using Core.Common.Utils;
using Core.Service.UseCase.GetState;
using Core.Service.UseCase.UpdateState;
using Core.Service.UseCase.WaitCommand;
using Core.Service.WebService.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Idler.WebService.Controllers {
	[ApiController]
	[Route("[controller]")]
	public sealed class StateController : ControllerBase {
		readonly ILogger<StateController>                  _logger;
		readonly GetStateUseCase<GameState>                _getStateUseCase;
		readonly UpdateStateUseCase<GameConfig, GameState> _updateStateUseCase;
		readonly WaitCommandUseCase<GameConfig, GameState> _waitCommandUseCase;

		public StateController(
			ILoggerFactory loggerFactory,
			GetStateUseCase<GameState> getStateUseCase,
			UpdateStateUseCase<GameConfig, GameState> updateStateUseCase,
			WaitCommandUseCase<GameConfig, GameState> waitCommandUseCase) {
			_logger             = loggerFactory.Create<StateController>();
			_getStateUseCase    = getStateUseCase;
			_updateStateUseCase = updateStateUseCase;
			_waitCommandUseCase = waitCommandUseCase;
		}

		[HttpPost("get")]
		public async Task<WebResponse> Get([FromBody] GetStateRequest request) {
			_logger.LogTrace($"Get state for user: '{request.UserId?.Value}'.");
			var response = await _getStateUseCase.Handle(request);
			_logger.LogTrace($"Response is '{response.GetType().Name}'.");
			return new WebResponse(response);
		}

		[HttpPost("update")]
		public async Task<WebResponse> Update([FromBody] UpdateStateRequest<GameConfig, GameState> request) {
			_logger.LogTrace($"Update state for user: '{request.UserId?.Value}'.");
			var response = await _updateStateUseCase.Handle(request);
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
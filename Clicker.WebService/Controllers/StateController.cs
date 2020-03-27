using System.Threading.Tasks;
using Clicker.Common.Config;
using Clicker.Common.State;
using Core.Common.Utils;
using Core.Service.UseCase.GetState;
using Core.Service.UseCase.UpdateState;
using Microsoft.AspNetCore.Mvc;

namespace Clicker.WebService.Controllers {
	[ApiController]
	[Route("[controller]")]
	public sealed class StateController : ControllerBase {
		readonly ILogger<StateController>                  _logger;
		readonly GetStateUseCase<GameState>                _getStateUseCase;
		readonly UpdateStateUseCase<GameConfig, GameState> _updateStateUseCase;

		public StateController(
			ILoggerFactory loggerFactory,
			GetStateUseCase<GameState> getStateUseCase, UpdateStateUseCase<GameConfig, GameState> updateStateUseCase) {
			_logger             = loggerFactory.Create<StateController>();
			_getStateUseCase    = getStateUseCase;
			_updateStateUseCase = updateStateUseCase;
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
	}
}
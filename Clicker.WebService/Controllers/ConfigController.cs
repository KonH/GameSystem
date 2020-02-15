using Clicker.Common.Config;
using Core.Common.Utils;
using Core.Service.UseCase.GetConfig;
using Microsoft.AspNetCore.Mvc;

namespace Clicker.WebService.Controllers {
	[ApiController]
	[Route("[controller]")]
	public sealed class ConfigController : ControllerBase {
		readonly ILogger<ConfigController>    _logger;
		readonly GetConfigUseCase<GameConfig> _getConfigUseCase;

		public ConfigController(ILoggerFactory loggerFactory, GetConfigUseCase<GameConfig> getConfigUseCase) {
			_logger           = loggerFactory.Create<ConfigController>();
			_getConfigUseCase = getConfigUseCase;
		}

		[HttpGet]
		public WebResponse Get([FromBody] GetConfigRequest request) {
			_logger.LogTrace($"Request config for user: '{request.UserId?.Value}'.");
			var response = _getConfigUseCase.Handle(request);
			_logger.LogTrace($"Response is '{response.GetType().Name}'.");
			return new WebResponse(response);
		}
	}
}
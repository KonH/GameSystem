using System.Threading.Tasks;
using Idler.Common.Config;
using Core.Common.Utils;
using Core.Service.UseCase.GetConfig;
using Core.Service.WebService.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Idler.WebService.Controllers {
	[ApiController]
	[Route("[controller]")]
	public sealed class ConfigController : ControllerBase {
		readonly ILogger<ConfigController>    _logger;
		readonly GetConfigUseCase<GameConfig> _getConfigUseCase;

		public ConfigController(ILoggerFactory loggerFactory, GetConfigUseCase<GameConfig> getConfigUseCase) {
			_logger           = loggerFactory.Create<ConfigController>();
			_getConfigUseCase = getConfigUseCase;
		}

		[HttpPost("get")]
		public async Task<WebResponse> Get([FromBody] GetConfigRequest request) {
			_logger.LogTrace($"Request config for user: '{request.UserId?.Value}'.");
			var response = await _getConfigUseCase.Handle(request);
			_logger.LogTrace($"Response is '{response.GetType().Name}'.");
			return new WebResponse(response);
		}
	}
}
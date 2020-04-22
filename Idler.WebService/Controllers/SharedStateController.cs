using Core.Common.Utils;
using Core.Service.WebService.Shared;
using Idler.Common.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Idler.WebService.Controllers {
	[ApiController]
	[Route("[controller]")]
	public sealed class SharedStateController : ControllerBase {
		readonly ILogger<SharedStateController> _logger;
		readonly ISharedStateRepository         _repository;

		public SharedStateController(ILoggerFactory loggerFactory, ISharedStateRepository repository) {
			_logger     = loggerFactory.Create<SharedStateController>();
			_repository = repository;
		}

		[HttpGet]
		public WebResponse Get() {
			var state = _repository.Get();
			_logger.LogTrace($"Return shared resources: '{state.Resources}'.");
			return new WebResponse(state);
		}
	}
}
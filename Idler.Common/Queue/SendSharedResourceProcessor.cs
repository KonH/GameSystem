using System.Threading;
using System.Threading.Tasks;
using Core.Common.Utils;
using Core.Service.Model;
using Idler.Common.Command;
using Idler.Common.Repository;

namespace Idler.Common.Queue {
	public sealed class SendSharedResourceProcessor {
		readonly ILogger<SendSharedResourceProcessor> _logger;
		readonly SharedStateRepository                _repository;

		SemaphoreSlim _semaphore = new SemaphoreSlim(1);

		public SendSharedResourceProcessor(ILoggerFactory loggerFactory, SharedStateRepository repository) {
			_logger     = loggerFactory.Create<SendSharedResourceProcessor>();
			_repository = repository;
		}

		public async Task Handle(UserId _, SendSharedResourceCommand __) {
			await _semaphore.WaitAsync();
			var state = _repository.Get();
			state.Resources++;
			_logger.LogTrace($"Shared resources now: {state.Resources}");
			_repository.Update(state);
			_semaphore.Release();
		}
	}
}
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Service.Queue;
using Idler.Common.Config;
using Idler.Common.State;
using Microsoft.Extensions.Hosting;

namespace Idler.WebService {
	sealed class UpdateCommandSchedulerService : IHostedService, IDisposable {
		readonly CommandScheduler<GameConfig, GameState> _scheduler;

		Timer _timer;

		public UpdateCommandSchedulerService(CommandScheduler<GameConfig, GameState> scheduler) {
			_scheduler = scheduler;
		}

		public Task StartAsync(CancellationToken cancellationToken) {
			_timer = new Timer(_ => Task.Run(_scheduler.Update, cancellationToken), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken) {
			_timer?.Change(Timeout.Infinite, 0);
			return Task.CompletedTask;
		}

		public void Dispose() {
			_timer?.Dispose();
		}
	}
}
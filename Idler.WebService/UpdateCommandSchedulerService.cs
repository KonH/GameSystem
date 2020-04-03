using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Service;
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
			_timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
			return Task.CompletedTask;
		}

		private void DoWork(object state) {
			_scheduler.Update();
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
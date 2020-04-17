using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Common.Config;
using Core.Common.State;
using Core.Service.Queue;
using Microsoft.Extensions.Hosting;

namespace Core.Service.WebService.Shared {
	public sealed class UpdateCommandSchedulerService<TConfig, TState> : IHostedService, IDisposable where TConfig : IConfig where TState : IState {
		readonly CommandScheduler<TConfig, TState> _scheduler;

		Timer _timer;

		public UpdateCommandSchedulerService(CommandScheduler<TConfig, TState> scheduler) {
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
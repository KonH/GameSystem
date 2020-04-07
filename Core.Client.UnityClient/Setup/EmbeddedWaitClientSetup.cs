using System;
using Core.Client.Abstractions;
using Core.Client.Embedded;
using Core.Client.UnityClient.DependencyInjection;
using Core.Common.Config;
using Core.Common.State;
using Core.Service.Queue;
using Core.Service.Shared;
using Core.Service.UseCase.WaitCommand;

namespace Core.Client.UnityClient.Setup {
	sealed class EmbeddedWaitClientSetup<TConfig, TState> : BaseEmbeddedClientSetup<TConfig, TState>
		where TConfig : class, IConfig where TState : class, IState, new() {

		public override void Configure(ServiceProvider provider) {
			base.Configure(provider);

			provider.AddService<CommandWorkQueue<TConfig, TState>>();
			provider.AddService(new WaitCommandSettings {
				WaitTime = TimeSpan.FromSeconds(60)
			});
			provider.AddService<CommandAwaiter<TConfig, TState>>();
			provider.AddService<WaitCommandUseCase<TConfig, TState>>();

			provider.AddService<IClient<TConfig, TState>, EmbeddedServiceWaitClient<TConfig, TState>>();

			provider.AddService<ITimeProvider, RealTimeProvider>();
			provider.AddService<CommandScheduler<TConfig, TState>>();
			provider.AddService<CommandScheduler<TConfig, TState>.Settings>();
		}
	}
}
using Core.Client.Abstractions;
using Core.Client.Shared;
using Core.Client.Standalone;
using Core.Client.UnityClient.DependencyInjection;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Client.UnityClient.Setup {
	sealed class StandaloneClientSetup<TConfig, TState> : CommonClientSetup<TConfig, TState>
		where TConfig : class, IConfig where TState : IState, new() {
		public override void Configure(ServiceProvider provider) {
			base.Configure(provider);

			provider.AddService<IClient<TConfig, TState>, StandaloneClient<TConfig, TState>>();
			provider.AddService(new StateFactory<TState>(() => new TState()));
		}
	}
}
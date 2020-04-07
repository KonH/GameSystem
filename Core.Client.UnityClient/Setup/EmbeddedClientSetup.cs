using Core.Client.Abstractions;
using Core.Client.Embedded;
using Core.Client.UnityClient.DependencyInjection;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Client.UnityClient.Setup {
	sealed class EmbeddedClientSetup<TConfig, TState> : BaseEmbeddedClientSetup<TConfig, TState>
		where TConfig : class, IConfig where TState : class, IState, new() {

		public override void Configure(ServiceProvider provider) {
			base.Configure(provider);

			provider.AddService<IClient<TConfig, TState>, EmbeddedServiceClient<TConfig, TState>>();
		}
	}
}
using Core.Client.Abstractions;
using Core.Client.UnityClient.DependencyInjection;
using Core.Client.Web;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Client.UnityClient.Setup {
	sealed class WebClientSetup<TConfig, TState> : BaseWebClientSetup<TConfig, TState>
		where TConfig : IConfig where TState : class, IState, new() {

		public override void Configure(ServiceProvider provider) {
			base.Configure(provider);

			provider.AddService<IClient<TConfig, TState>, WebServiceClient<TConfig, TState>>();
		}
	}
}
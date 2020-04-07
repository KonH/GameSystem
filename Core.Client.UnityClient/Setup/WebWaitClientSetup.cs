using Core.Client.Abstractions;
using Core.Client.UnityClient.DependencyInjection;
using Core.Client.UnityClient.Threading;
using Core.Client.Web;
using Core.Common.Config;
using Core.Common.State;
using Core.Common.Threading;

namespace Core.Client.UnityClient.Setup {
	sealed class WebWaitClientSetup<TConfig, TState> : BaseWebClientSetup<TConfig, TState>
		where TConfig : IConfig where TState : class, IState, new() {

		public override void Configure(ServiceProvider provider) {
			base.Configure(provider);

			provider.AddService<IClient<TConfig, TState>, WebServiceWaitClient<TConfig, TState>>();

			provider.AddService<ITaskRunner, UnityTaskRunner>();
		}
	}
}
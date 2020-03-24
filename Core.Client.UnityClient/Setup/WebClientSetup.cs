using Core.Client.Abstractions;
using Core.Client.Shared;
using Core.Client.UnityClient.DependencyInjection;
using Core.Client.UnityClient.Shared;
using Core.Client.UnityClient.Utils;
using Core.Client.Utils;
using Core.Client.Web;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Client.UnityClient.Setup {
	sealed class WebClientSetup<TConfig, TState> : CommonClientSetup<TConfig, TState>
		where TConfig : IConfig where TState : class, IState, new() {

		public override void Configure(ServiceProvider provider) {
			base.Configure(provider);

			provider.AddService<IRequestSerializer, NewtonsoftJsonRequestSerializer>();
			provider.AddService<IWebRequestHandler, UnityWebRequestHandler>();
			provider.AddService<WebClientHandler>();

			provider.AddService<ISettingsSource, PersistentSettingsSource>();
			provider.AddService<UserIdGenerator>();
			provider.AddService<UserIdSource>();
			provider.AddService<IClient<TConfig, TState>, WebServiceClient<TConfig, TState>>();
		}
	}
}
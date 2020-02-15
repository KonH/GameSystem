using Core.Client.Abstractions;
using Core.Client.Web;
using Core.Client.WebClient;
using Core.Common.CommandDependency;
using Core.Common.Config;
using Core.Common.State;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Client.ConsoleClient.Setup {
	sealed class WebClientSetup<TConfig, TState> : CommonClientSetup<TConfig, TState>
		where TConfig : IConfig where TState : class, IState, new() {
		readonly string _baseUrl;

		public WebClientSetup(CommandQueue<TConfig, TState> queue, string baseUrl) : base(queue) {
			_baseUrl = baseUrl;
		}

		public WebClientSetup(CommandQueue<TConfig, TState> queue) : base(queue) { }

		public override void Configure(ServiceCollection services) {
			base.Configure(services);

			services.AddSingleton<IRequestSerializer, NewtonsoftJsonRequestSerializer>();
			services.AddSingleton<IWebRequestHandler>(new StandardWebRequestHandler(_baseUrl));
			services.AddSingleton<WebClientHandler>();

			services.AddSingleton<IClient<TConfig, TState>, WebServiceClient<TConfig, TState>>();
		}

		public override void Initialize(ServiceProvider provider) { }
	}
}
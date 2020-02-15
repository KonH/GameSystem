using Core.Client.Abstractions;
using Core.Client.Shared;
using Core.Client.Standalone;
using Core.Common.CommandDependency;
using Core.Common.Config;
using Core.Common.State;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Client.ConsoleClient.Setup {
	sealed class StandaloneClientSetup<TConfig, TState> : CommonClientSetup<TConfig, TState>
		where TConfig : class, IConfig where TState : IState, new() {
		readonly TConfig _config;

		public StandaloneClientSetup(CommandQueue<TConfig, TState> queue, TConfig config) : base(queue) {
			_config = config;
		}

		public override void Configure(ServiceCollection services) {
			base.Configure(services);

			services.AddSingleton(_config);
			services.AddSingleton<IClient<TConfig, TState>, StandaloneClient<TConfig, TState>>();

			services.AddSingleton(new StateFactory<TState>(() => new TState()));
		}

		public override void Initialize(ServiceProvider provider) { }
	}
}
using Core.Client.Abstractions;
using Core.Client.Embedded;
using Core.Client.Shared;
using Core.Common.CommandDependency;
using Core.Common.Config;
using Core.Common.State;
using Core.Service.Extension;
using Core.Service.Model;
using Core.Service.Repository;
using Core.Service.Repository.Config;
using Core.Service.Repository.State;
using Core.Service.UseCase.GetConfig;
using Core.Service.UseCase.GetState;
using Core.Service.UseCase.UpdateState;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Client.ConsoleClient.Setup {
	sealed class EmbeddedClientSetup<TConfig, TState> : CommonClientSetup<TConfig, TState>
		where TConfig : IConfig where TState : class, IState, new() {
		readonly TConfig _config;

		public EmbeddedClientSetup(CommandQueue<TConfig, TState> queue, TConfig config): base(queue) {
			_config = config;
		}

		public override void Configure(ServiceCollection services) {
			base.Configure(services);

			services.AddSingleton(JsonRepositoryDecoratorSettings.Create<TConfig>());
			services.AddSingleton<IConfigRepository<TConfig>, InMemoryConfigRepository<TConfig>>();

			services.AddSingleton(JsonRepositoryDecoratorSettings.Create<TState>());
			services.AddSingleton<IStateRepository<TState>, InMemoryStateRepository<TState>>();

			services.AddSingleton(new GetSingleConfigStrategy<TConfig>.Settings(_config.Version));
			services.AddSingleton<IGetConfigStrategy<TConfig>, GetSingleConfigStrategy<TConfig>>();
			services.AddSingleton<GetConfigUseCase<TConfig>>();

			services.AddSingleton<GetStateUseCase<TState>>();

			services.AddSingleton<UpdateStateUseCase<TConfig, TState>>();

			services.AddSingleton<IClient<TConfig, TState>, EmbeddedServiceClient<TConfig, TState>>();

			services.AddSingleton(new StateFactory<TState>(() => new TState()));
		}

		public override void Initialize(ServiceProvider provider) {
			provider.GetRequiredService<IConfigRepository<TConfig>>()
				.Add(_config);
		}
	}
}
using Core.Client.Shared;
using Core.Client.UnityClient.DependencyInjection;
using Core.Client.UnityClient.Shared;
using Core.Common.Config;
using Core.Common.State;
using Core.Service.Extension;
using Core.Service.Repository;
using Core.Service.Repository.Config;
using Core.Service.Repository.State;
using Core.Service.UseCase.GetConfig;
using Core.Service.UseCase.GetState;
using Core.Service.UseCase.SendCommand;

namespace Core.Client.UnityClient.Setup {
	abstract class BaseEmbeddedClientSetup<TConfig, TState> : CommonClientSetup<TConfig, TState>
		where TConfig : class, IConfig where TState : class, IState, new() {

		public override void Configure(ServiceProvider provider) {
			base.Configure(provider);

			provider.AddService(JsonRepositoryDecoratorSettings.Create<TConfig>());
			provider.AddService<IConfigRepository<TConfig>, InMemoryConfigRepository<TConfig>>();

			provider.AddService(JsonRepositoryDecoratorSettings.Create<TState>());
			provider.AddService<IStateRepository<TState>, InMemoryStateRepository<TState>>();

			var config = provider.GetService<TConfig>();
			provider.AddService(new GetSingleConfigStrategy<TConfig>.Settings(config.Version));
			provider.AddService<IGetConfigStrategy<TConfig>, GetSingleConfigStrategy<TConfig>>();
			provider.AddService<GetConfigUseCase<TConfig>>();

			provider.AddService<GetStateUseCase<TState>>();

			provider.AddService<SendCommandUseCase<TConfig, TState>>();

			provider.AddService(new StateFactory<TState>(() => new TState()));

			provider.GetService<IConfigRepository<TConfig>>()
				.Add(config);

			provider.AddService<ISettingsSource, PersistentSettingsSource>();
			provider.AddService<UserIdGenerator>();
			provider.AddService<UserIdSource>();
		}
	}
}
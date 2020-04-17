using System;
using Core.Client.Abstractions;
using Core.Client.ConsoleClient.Shared;
using Core.Client.Embedded;
using Core.Client.Shared;
using Core.Common.CommandDependency;
using Core.Common.Config;
using Core.Common.State;
using Core.Service.Extension;
using Core.Service.Queue;
using Core.Service.Repository;
using Core.Service.Repository.Config;
using Core.Service.Repository.State;
using Core.Service.UseCase.GetConfig;
using Core.Service.UseCase.GetState;
using Core.Service.UseCase.SendCommand;
using Core.Service.UseCase.WaitCommand;
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

			services.AddSingleton<CommonWatcher<TConfig, TState>>();
			services.AddSingleton(sp => {
				var settings = new CommandScheduler<TConfig, TState>.Settings();
				settings.AddWatcher(sp.GetService<CommonWatcher<TConfig, TState>>());
				return settings;
			});
			services.AddSingleton<SendCommandUseCase<TConfig, TState>>();

			services.AddSingleton<CommandAwaiter<TConfig, TState>>();
			services.AddSingleton<CommandWorkQueue<TConfig, TState>>();
			services.AddSingleton<CommandScheduler<TConfig, TState>>();
			services.AddSingleton(new WaitCommandSettings {
				WaitTime = TimeSpan.FromSeconds(30)
			});
			services.AddSingleton<WaitCommandUseCase<TConfig, TState>>();

			services.AddSingleton<ISettingsSource, FileSettingsSource>();
			services.AddSingleton<UserIdGenerator>();
			services.AddSingleton<UserIdSource>();
			services.AddSingleton<IClient<TConfig, TState>, EmbeddedServiceWaitClient<TConfig, TState>>();

			services.AddSingleton(new StateFactory<TState>(() => new TState()));
		}

		public override void Initialize(ServiceProvider provider) {
			provider.GetRequiredService<IConfigRepository<TConfig>>()
				.Add(_config);
		}
	}
}
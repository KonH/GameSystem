using System;
using System.Collections.Generic;
using Clicker.Common;
using Clicker.Common.Config;
using Clicker.Common.State;
using Core.Client;
using Core.Common.CommandDependency;
using Core.Common.CommandExecution;
using Core.Common.Utils;
using Core.Client.ConsoleClient;
using Core.Client.ConsoleClient.Utils;
using Core.Common.Config;
using Core.Service.Extension;
using Core.Service.Model;
using Core.Service.Repository.Config;
using Core.Service.Repository.State;
using Core.Service.UseCase.GetConfig;
using Core.Service.UseCase.GetState;
using Core.Service.UseCase.UpdateState;
using Microsoft.Extensions.DependencyInjection;

namespace Clicker.ConsoleClient {
	static class Program {
		static readonly Dictionary<string, Func<ServiceCollection, ServiceProvider>> DependenciesByType =
			new Dictionary<string, Func<ServiceCollection, ServiceProvider>> {
				{ "embedded", AddEmbeddedDependencies }
			};

		static GameConfig Config => new GameConfig {
			Version = new ConfigVersion("Config"),
			Resource = new ResourceConfig {
				ResourceByClick = 10
			},
			Upgrade = new UpgradeConfig {
				Levels = new[] {
					new UpgradeLevel {
						Cost  = 30,
						Power = 2.0
					},
					new UpgradeLevel {
						Cost  = 100,
						Power = 3.0
					},
					new UpgradeLevel {
						Cost  = 200,
						Power = 5.0
					}
				}
			}
		};

		static void Main(string[] args) {
			using var provider = Configure((args.Length > 0) ? args[0] : string.Empty);
			Run(provider);
		}

		static ServiceProvider Configure(string mode) {
			var services = new ServiceCollection();
			AddCommonDependencies(services);
			AddGameDependencies(services);
			var addSpecificDependencies = DependenciesByType.GetValueOrDefault(mode) ?? AddStandaloneDependencies;
			return addSpecificDependencies(services);
		}

		static void Run(ServiceProvider serviceProvider) {
			var client = serviceProvider.GetRequiredService<ConsoleRunner<GameConfig, GameState>>();
			client.Run();
		}

		static void AddCommonDependencies(ServiceCollection services) {
			services.AddSingleton<BatchCommandExecutor<GameConfig, GameState>>();
			services.AddSingleton(new TypeLoggerFactory(typeof(ConsoleLogger<>)));
			services.AddSingleton<ConsoleReader>();
			services.AddSingleton<ConsoleRunner<GameConfig, GameState>>();
		}

		static void AddGameDependencies(ServiceCollection services) {
			services.AddSingleton(new CommandProvider<GameConfig, GameState>(typeof(GameState).Assembly));
			services.AddSingleton<CommandQueue<GameConfig, GameState>, CommandQueue>();
			services.AddSingleton(new StateFactory<GameState>(() => new GameState()));
		}

		static ServiceProvider AddStandaloneDependencies(ServiceCollection services) {
			services.AddSingleton(Config);
			services.AddSingleton<IClient<GameConfig, GameState>, StandaloneClient<GameConfig, GameState>>();

			return services.BuildServiceProvider();
		}

		static ServiceProvider AddEmbeddedDependencies(ServiceCollection services) {
			services.AddSingleton(JsonRepositoryDecoratorSettings.Create<GameConfig>());
			services.AddSingleton<IConfigRepository<GameConfig>, InMemoryConfigRepository<GameConfig>>();
			services.AddSingleton(JsonRepositoryDecoratorSettings.Create<GameState>());
			services.AddSingleton<IStateRepository<GameState>, InMemoryStateRepository<GameState>>();
			services.AddSingleton(new GetSingleConfigStrategy<GameConfig>.Settings(new ConfigVersion("Config")));
			services.AddSingleton<IGetConfigStrategy<GameConfig>, GetSingleConfigStrategy<GameConfig>>();
			services.AddSingleton<GetConfigUseCase<GameConfig>>();
			services.AddSingleton<GetStateUseCase<GameState>>();
			services.AddSingleton<UpdateStateUseCase<GameConfig, GameState>>();
			services.AddSingleton<IClient<GameConfig, GameState>, EmbeddedServiceClient<GameConfig, GameState>>();

			var provider = services.BuildServiceProvider();
			provider.GetRequiredService<IConfigRepository<GameConfig>>()
				.Add(Config);
			provider.GetRequiredService<IStateRepository<GameState>>()
				.Add(new UserId("UserId"), new GameState());

			return provider;
		}
	}
}
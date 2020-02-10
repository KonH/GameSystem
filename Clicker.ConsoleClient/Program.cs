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
using Core.Service.Repository.Config;
using Core.Service.Repository.State;
using Core.Service.UseCase.GetState;
using Core.Service.UseCase.UpdateState;
using Microsoft.Extensions.DependencyInjection;

namespace Clicker.ConsoleClient {
	static class Program {
		static readonly Dictionary<string, Action<ServiceCollection>> DependenciesByType =
			new Dictionary<string, Action<ServiceCollection>> {
				{ "embedded", AddEmbeddedDependencies }
			};

		static GameConfig Config => new GameConfig {
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
			var services = Configure((args.Length > 0) ? args[0] : string.Empty);
			using var provider = services.BuildServiceProvider();
			Run(provider);
		}

		static ServiceCollection Configure(string mode) {
			var services = new ServiceCollection();
			AddCommonDependencies(services);
			AddGameDependencies(services);
			var addSpecificDependencies = DependenciesByType.GetValueOrDefault(mode) ?? AddStandaloneDependencies;
			addSpecificDependencies(services);
			return services;
		}

		static void Run(ServiceProvider serviceProvider) {
			var client = serviceProvider.GetRequiredService<ConsoleRunner<GameConfig, GameState>>();
			client.Run();
		}

		static void AddCommonDependencies(ServiceCollection services) {
			services.AddSingleton<BatchCommandExecutor<GameConfig, GameState>>();
			services.AddSingleton(new LoggerFactory(typeof(ConsoleLogger<>)));
			services.AddSingleton<ConsoleReader>();
			services.AddSingleton<ConsoleRunner<GameConfig, GameState>>();
		}

		static void AddGameDependencies(ServiceCollection services) {
			services.AddSingleton(Config);
			services.AddSingleton(new CommandProvider<GameConfig, GameState>(typeof(GameState).Assembly));
			services.AddSingleton<CommandQueue<GameConfig, GameState>, CommandQueue>();
			services.AddSingleton(new StateFactory<GameState>(() => new GameState()));
		}

		static void AddStandaloneDependencies(ServiceCollection services) {
			services.AddSingleton<StandaloneClient<GameConfig, GameState>>();
			services.AddSingleton<IConsoleClient<GameConfig, GameState>, StandaloneConsoleClient<GameConfig, GameState>>();
		}

		static void AddEmbeddedDependencies(ServiceCollection services) {
			services.AddSingleton(RepositoryDecoratorSettings.Create<GameConfig>());
			services.AddSingleton<IConfigRepository<GameConfig>, InMemoryConfigRepository<GameConfig>>();
			services.AddSingleton(RepositoryDecoratorSettings.Create<GameState>());
			services.AddSingleton<IStateRepository<GameState>, InMemoryStateRepository<GameState>>();
			services.AddSingleton<GetStateUseCase<GameState>>();
			services.AddSingleton<UpdateStateUseCase<GameConfig, GameState>>();
			services.AddSingleton<EmbeddedServiceClient<GameConfig, GameState>>();
			services.AddSingleton<IConsoleClient<GameConfig, GameState>, EmbeddedServiceConsoleClient<GameConfig, GameState>>();
		}
	}
}
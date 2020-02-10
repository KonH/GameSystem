using Clicker.Common;
using Clicker.Common.Config;
using Clicker.Common.State;
using Core.Client;
using Core.Common.CommandDependency;
using Core.ConsoleClient;
using Microsoft.Extensions.DependencyInjection;

namespace Clicker.ConsoleClient {
	static class Program {
		static void Main() {
			var services = Configure();
			using var provider = services.BuildServiceProvider();
			Run(provider);
		}

		static ServiceCollection Configure() {
			var services = new ServiceCollection();
			services.AddSingleton<CommandQueue<GameConfig, GameState>, CommandQueue>();
			services.AddSingleton(CreateConfig());
			services.AddSingleton(new StateFactory<GameState>(() => new GameState()));
			services.AddSingleton(new CommandProvider<GameConfig, GameState>(typeof(GameState).Assembly));
			services.AddSingleton<StandaloneConsoleClient<GameConfig, GameState>>();
			return services;
		}

		static void Run(ServiceProvider serviceProvider) {
			var client = serviceProvider.GetRequiredService<StandaloneConsoleClient<GameConfig, GameState>>();
			client.Run();
		}

		static GameConfig CreateConfig() {
			return new GameConfig {
				Resource = new ResourceConfig {
					ResourceByClick = 10
				},
				Upgrade = new UpgradeConfig {
					Levels = new[] {
						new UpgradeLevel {
							Cost = 30,
							Power = 2.0
						},
						new UpgradeLevel {
							Cost = 100,
							Power = 3.0
						},
						new UpgradeLevel {
							Cost = 200,
							Power = 5.0
						}
					}
				}
			};
		}
	}
}
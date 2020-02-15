using System.Threading.Tasks;
using Clicker.Common;
using Clicker.Common.Config;
using Clicker.Common.State;
using Core.Client.ConsoleClient.Setup;
using Core.Common.Config;
using Microsoft.Extensions.DependencyInjection;

namespace Clicker.ConsoleClient {
	static class Program {
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

		static async Task Main(string[] args) {
			var setup = SetupFactory<GameConfig, GameState>.CreateByArguments(new CommandQueue(), Config, args);
			var services = new ServiceCollection();
			setup.Configure(services);
			var provider = services.BuildServiceProvider();
			setup.Initialize(provider);
			await setup.Run(provider);
		}
	}
}
using System.Threading.Tasks;
using Idler.Common;
using Idler.Common.Config;
using Idler.Common.State;
using Core.Client.ConsoleClient.Setup;
using Core.Common.Config;
using Microsoft.Extensions.DependencyInjection;

namespace Idler.ConsoleClient {
	static class Program {
		static GameConfig Config => new GameConfig {
			Version = new ConfigVersion("Config"),
			Resource = new ResourceConfig {
				ResourceByTick = 10,
				SharedCost     = 100,
			},
			Time = new TimeConfig {
				TickInterval = 15
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
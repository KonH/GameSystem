using Clicker.Common;
using Clicker.Common.Config;
using Clicker.Common.State;
using Core.Common.Command;
using Core.Common.Utils;
using Core.ConsoleClient;

namespace Clicker.ConsoleClient {
	class Program {
		static void Main() {
			GameState createState() => new GameState();
			var commands = TypeResolver.GetSubclasses<ICommand<GameConfig, GameState>>(typeof(GameConfig).Assembly);
			var config = CreateConfig();
			var client = new StandaloneConsoleClient<GameConfig, GameState>(
				commands, new CommandQueue(), config, createState);
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
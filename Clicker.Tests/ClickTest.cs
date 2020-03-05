using System.Threading.Tasks;
using Clicker.Common.Command;
using Clicker.Common.Config;
using Clicker.Common.State;
using NUnit.Framework;

namespace Clicker.Tests {
	public sealed class ClickTest {
		[Test]
		public async Task IsClickCountIncreased() {
			var executor = Common.CreateExecutor();
			var state    = new GameState();

			await executor.Apply(new GameConfig(), state, new ClickCommand());

			Assert.AreEqual(1, state.Click.Clicks);
		}

		[Test]
		public async Task IsResourceAdded() {
			var executor = Common.CreateExecutor();
			var config = new GameConfig {
				Resource = new ResourceConfig {
					ResourceByClick = 10
				}
			};
			var state = new GameState();

			await executor.Apply(config, state, new ClickCommand());

			Assert.AreEqual(10, state.Resource.Resources);
		}

		[Test]
		public async Task IsUpgradeLevelIncreaseResourceCount() {
			var executor = Common.CreateExecutor();
			var config = new GameConfig {
				Resource = new ResourceConfig {
					ResourceByClick = 10
				},
				Upgrade = new UpgradeConfig {
					Levels = new[] {
						new UpgradeLevel {
							Power = 2.0
						},
					}
				}
			};
			var state = new GameState {
				Upgrade = new UpgradeState {
					Level = 1
				}
			};

			await executor.Apply(config, state, new ClickCommand());

			Assert.AreEqual(20, state.Resource.Resources);
		}
	}
}
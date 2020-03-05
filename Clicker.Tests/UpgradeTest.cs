using System.Threading.Tasks;
using Clicker.Common.Command;
using Clicker.Common.Config;
using Clicker.Common.State;
using Core.Common.CommandExecution;
using NUnit.Framework;

namespace Clicker.Tests {
	public sealed class UpgradeTest {
		[Test]
		public async Task IsUpgradeIncreaseUpgradeLevel() {
			var executor = Common.CreateExecutor();
			var config   = CreateConfig();
			var state = new GameState {
				Resource = new ResourceState {
					Resources = 10
				}
			};

			await executor.Apply(config, state, new UpgradeCommand());

			Assert.AreEqual(1, state.Upgrade.Level);
		}

		[Test]
		public async Task IsUpgradeDecreaseResources() {
			var executor = Common.CreateExecutor();
			var config   = CreateConfig();
			var state = new GameState {
				Resource = new ResourceState {
					Resources = 10
				}
			};

			await executor.Apply(config, state, new UpgradeCommand());

			Assert.AreEqual(0, state.Resource.Resources);
		}

		[Test]
		public async Task IsUpgradeFailedIfNotEnoughResources() {
			var executor = Common.CreateExecutor();
			var config   = CreateConfig();

			var result = await executor.Apply(config, new GameState(), new UpgradeCommand());

			Assert.IsInstanceOf<BatchCommandResult.BadCommand>(result);
		}


		[Test]
		public async Task CantUpgradeToUnknownLevel() {
			var executor = Common.CreateExecutor();
			var config   = new GameConfig();
			var state = new GameState {
				Upgrade = new UpgradeState {
					Level = 0
				}
			};

			var result = await executor.Apply(config, state, new UpgradeCommand());

			Assert.IsInstanceOf<BatchCommandResult.BadCommand>(result);
		}

		GameConfig CreateConfig() {
			return new GameConfig {
				Upgrade = new UpgradeConfig {
					Levels = new[] {
						new UpgradeLevel {
							Cost = 10
						}
					}
				}
			};
		}
	}
}
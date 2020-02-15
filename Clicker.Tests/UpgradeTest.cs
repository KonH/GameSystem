using Clicker.Common.Command;
using Clicker.Common.Config;
using Clicker.Common.State;
using Core.Common.CommandExecution;
using NUnit.Framework;

namespace Clicker.Tests {
	public sealed class UpgradeTest {
		[Test]
		public void IsUpgradeIncreaseUpgradeLevel() {
			var executor = Common.CreateExecutor();
			var config   = CreateConfig();
			var state = new GameState {
				Resource = new ResourceState {
					Resources = 10
				}
			};

			executor.Apply(config, state, new UpgradeCommand());

			Assert.AreEqual(1, state.Upgrade.Level);
		}

		[Test]
		public void IsUpgradeDecreaseResources() {
			var executor = Common.CreateExecutor();
			var config   = CreateConfig();
			var state = new GameState {
				Resource = new ResourceState {
					Resources = 10
				}
			};

			executor.Apply(config, state, new UpgradeCommand());

			Assert.AreEqual(0, state.Resource.Resources);
		}

		[Test]
		public void IsUpgradeFailedIfNotEnoughResources() {
			var executor = Common.CreateExecutor();
			var config   = CreateConfig();

			var result = executor.Apply(config, new GameState(), new UpgradeCommand());

			Assert.IsInstanceOf<BatchCommandResult.BadCommand>(result);
		}


		[Test]
		public void CantUpgradeToUnknownLevel() {
			var executor = Common.CreateExecutor();
			var config   = new GameConfig();
			var state = new GameState {
				Upgrade = new UpgradeState {
					Level = 1
				}
			};

			var result = executor.Apply(config, state, new UpgradeCommand());

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
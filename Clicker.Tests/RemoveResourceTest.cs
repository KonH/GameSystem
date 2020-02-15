using Clicker.Common.Command;
using Clicker.Common.Config;
using Clicker.Common.State;
using Core.Common.CommandExecution;
using NUnit.Framework;

namespace Clicker.Tests {
	public sealed class RemoveResourceTest {
		[Test]
		public void IsResourceRemoved() {
			var executor = Common.CreateExecutor();
			var state = new GameState {
				Resource = new ResourceState {
					Resources = 10
				}
			};

			executor.Apply(new GameConfig(), state, new RemoveResourceCommand(10));

			Assert.AreEqual(0, state.Resource.Resources);
		}

		[Test]
		public void IsResourceNotRemovedIfNotEnough() {
			var executor = Common.CreateExecutor();

			var result = executor.Apply(new GameConfig(), new GameState(), new RemoveResourceCommand(10));

			Assert.IsInstanceOf<BatchCommandResult.BadCommand>(result);
		}

		[Test]
		public void IsZeroCountNotRemoved() {
			var executor = Common.CreateExecutor();
			var state = new GameState {
				Resource = new ResourceState {
					Resources = 10
				}
			};

			var result = executor.Apply(new GameConfig(), state, new RemoveResourceCommand());

			Assert.IsInstanceOf<BatchCommandResult.BadCommand>(result);
		}

		[Test]
		public void IsInvalidCountNotRemoved() {
			var executor = Common.CreateExecutor();
			var state = new GameState {
				Resource = new ResourceState {
					Resources = 10
				}
			};

			var result = executor.Apply(new GameConfig(), state, new RemoveResourceCommand(-10));

			Assert.IsInstanceOf<BatchCommandResult.BadCommand>(result);
		}
	}
}
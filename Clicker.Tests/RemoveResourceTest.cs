using System.Threading.Tasks;
using Clicker.Common.Command;
using Clicker.Common.Config;
using Clicker.Common.State;
using Core.Common.CommandExecution;
using NUnit.Framework;

namespace Clicker.Tests {
	public sealed class RemoveResourceTest {
		[Test]
		public async Task IsResourceRemoved() {
			var executor = Common.CreateExecutor();
			var state = new GameState {
				Resource = new ResourceState {
					Resources = 10
				}
			};

			await executor.Apply(new GameConfig(), state, new RemoveResourceCommand(10));

			Assert.AreEqual(0, state.Resource.Resources);
		}

		[Test]
		public async Task IsResourceNotRemovedIfNotEnough() {
			var executor = Common.CreateExecutor();

			var result = await executor.Apply(new GameConfig(), new GameState(), new RemoveResourceCommand(10));

			Assert.IsInstanceOf<BatchCommandResult.BadCommand>(result);
		}

		[Test]
		public async Task IsZeroCountNotRemoved() {
			var executor = Common.CreateExecutor();
			var state = new GameState {
				Resource = new ResourceState {
					Resources = 10
				}
			};

			var result = await executor.Apply(new GameConfig(), state, new RemoveResourceCommand());

			Assert.IsInstanceOf<BatchCommandResult.BadCommand>(result);
		}

		[Test]
		public async Task IsInvalidCountNotRemoved() {
			var executor = Common.CreateExecutor();
			var state = new GameState {
				Resource = new ResourceState {
					Resources = 10
				}
			};

			var result = await executor.Apply(new GameConfig(), state, new RemoveResourceCommand(-10));

			Assert.IsInstanceOf<BatchCommandResult.BadCommand>(result);
		}
	}
}
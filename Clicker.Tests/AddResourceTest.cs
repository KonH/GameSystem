using System.Threading.Tasks;
using Clicker.Common.Command;
using Clicker.Common.Config;
using Clicker.Common.State;
using Core.Common.CommandExecution;
using NUnit.Framework;

namespace Clicker.Tests {
	public sealed class AddResourceTest {
		[Test]
		public async Task IsResourceAdded() {
			var executor = Common.CreateExecutor();
			var state    = new GameState();

			await executor.Apply(new GameConfig(), state, new AddResourceCommand(10));

			Assert.AreEqual(10, state.Resource.Resources);
		}

		[Test]
		public async Task IsZeroCountNotAdded() {
			var executor = Common.CreateExecutor();

			var result = await executor.Apply(new GameConfig(), new GameState(), new AddResourceCommand());

			Assert.IsInstanceOf<BatchCommandResult.BadCommand>(result);
		}

		[Test]
		public async Task IsInvalidCountNotAdded() {
			var executor = Common.CreateExecutor();

			var result = await executor.Apply(new GameConfig(), new GameState(), new AddResourceCommand(-10));

			Assert.IsInstanceOf<BatchCommandResult.BadCommand>(result);
		}
	}
}
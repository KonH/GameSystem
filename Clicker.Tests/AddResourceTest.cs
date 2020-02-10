using Clicker.Common.Command;
using Clicker.Common.Config;
using Clicker.Common.State;
using Core.Common.CommandExecution;
using NUnit.Framework;

namespace Clicker.Tests {
	public sealed class AddResourceTest {
		[Test]
		public void IsResourceAdded() {
			var executor = Common.CreateExecutor();
			var state = new GameState();

			executor.Apply(new GameConfig(), state, new AddResourceCommand { Amount = 10 });

			Assert.AreEqual(10, state.Resource.Resources);
		}

		[Test]
		public void IsZeroCountNotAdded() {
			var executor = Common.CreateExecutor();

			var result = executor.Apply(new GameConfig(), new GameState(), new AddResourceCommand());

			Assert.IsInstanceOf<BatchCommandResult<GameConfig, GameState>.BadCommand>(result);
		}

		[Test]
		public void IsInvalidCountNotAdded() {
			var executor = Common.CreateExecutor();

			var result = executor.Apply(new GameConfig(), new GameState(), new AddResourceCommand { Amount = -10 });

			Assert.IsInstanceOf<BatchCommandResult<GameConfig, GameState>.BadCommand>(result);
		}
	}
}
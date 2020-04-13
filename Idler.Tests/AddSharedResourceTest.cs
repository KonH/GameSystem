using System.Threading.Tasks;
using Idler.Common.Command;
using Idler.Common.Config;
using Idler.Common.State;
using Core.Common.CommandExecution;
using NUnit.Framework;

namespace Idler.Tests {
	public sealed class AddSharedResourceTest {
		[Test]
		public async Task IsResourceAdded() {
			var executor = Common.CreateExecutor();
			var state    = new GameState();
			state.Resource.Resources = 100;

			await executor.Apply(new GameConfig(), state, new AddSharedResourceCommand());

			Assert.AreEqual(1, state.Resource.SharedResources);
		}

		[Test]
		public async Task IsNotAddedIfNotEnoughResources() {
			var executor = Common.CreateExecutor();

			var result = await executor.Apply(new GameConfig(), new GameState(), new AddResourceCommand());

			Assert.IsInstanceOf<BatchCommandResult.BadCommand>(result);
		}
	}
}
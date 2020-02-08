using Clicker.Common;
using Clicker.Common.Config;
using Clicker.Common.State;
using Core.Common.CommandExecution;
using Core.Common.Tests.Utils;

namespace Clicker.Tests {
	public static class Common {
		public static BatchCommandExecutor<GameConfig, GameState> CreateExecutor() {
			var logger = new TestLogger<BatchCommandExecutor<GameConfig, GameState>>();
			var queue = new CommandQueue().Queue;
			return new BatchCommandExecutor<GameConfig, GameState>(logger, queue);
		}
	}
}
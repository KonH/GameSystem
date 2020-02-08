using Clicker.Common;
using Clicker.Common.Config;
using Clicker.Common.State;
using Core.Common.CommandExecution;
using Core.Common.Utils;

namespace Clicker.Tests {
	public static class Common {
		public static BatchCommandExecutor<GameConfig, GameState> CreateExecutor() {
			var logger = new ConsoleLogger<BatchCommandExecutor<GameConfig, GameState>>();
			var queue = new CommandQueue();
			return new BatchCommandExecutor<GameConfig, GameState>(logger, queue);
		}
	}
}
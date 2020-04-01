using Clicker.Common;
using Idler.Common.Config;
using Idler.Common.State;
using Core.Common.CommandExecution;
using Core.Common.Utils;

namespace Idler.Tests {
	public static class Common {
		public static BatchCommandExecutor<GameConfig, GameState> CreateExecutor() {
			var loggerFactory = new TypeLoggerFactory(typeof(ConsoleLogger<>));
			var queue         = new CommandQueue();
			return new BatchCommandExecutor<GameConfig, GameState>(loggerFactory, new CommandExecutor<GameConfig, GameState>(), queue);
		}
	}
}
using Clicker.Common;
using Clicker.Common.Config;
using Clicker.Common.State;
using Core.Common.CommandExecution;
using Core.Common.Utils;

namespace Clicker.Tests {
	public static class Common {
		public static BatchCommandExecutor<GameConfig, GameState> CreateExecutor() {
			var loggerFactory = new TypeLoggerFactory(typeof(ConsoleLogger<>));
			var queue         = new CommandQueue();
			return new BatchCommandExecutor<GameConfig, GameState>(loggerFactory, new CommandExecutor<GameConfig, GameState>(loggerFactory), queue);
		}
	}
}
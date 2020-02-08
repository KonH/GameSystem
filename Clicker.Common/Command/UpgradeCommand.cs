using Clicker.Common.Config;
using Clicker.Common.State;
using Core.Common.Command;

namespace Clicker.Common.Command {
	public sealed class UpgradeCommand : ICommand<GameConfig, GameState> {
		public CommandResult Apply(GameConfig config, GameState state) {
			var levelCount = config.Upgrade.Levels.Length;
			var currentLevel = state.Upgrade.Level;
			var nextLevel = currentLevel + 1;
			if ( nextLevel > levelCount ) {
				return CommandResult.BadCommand("No more available upgrade levels");
			}
			state.Upgrade.Level = nextLevel;
			return CommandResult.Ok();
		}
	}
}
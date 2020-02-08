using Clicker.Common.Command;
using Clicker.Common.Config;
using Clicker.Common.State;

namespace Clicker.Common.Handler {
	public static class RemoveResource {
		public static RemoveResourceCommand Create(GameConfig config, GameState state, UpgradeCommand _) {
			var upgradeLevel = state.Upgrade.Level;
			var levelCount = config.Upgrade.Levels.Length;
			var amount = (upgradeLevel <= levelCount) ? config.Upgrade.Levels[upgradeLevel - 1].Cost : 0;
			return new RemoveResourceCommand { Amount = amount };
		}
	}
}
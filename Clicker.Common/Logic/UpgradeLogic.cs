using Clicker.Common.Config;
using Clicker.Common.State;

namespace Clicker.Common.Logic {
	public static class UpgradeLogic {
		public static bool HasNextUpgradeLevel(GameConfig config, GameState state) {
			var levelCount   = config.Upgrade.Levels.Length;
			var currentLevel = state.Upgrade.Level;
			return (currentLevel < levelCount);
		}

		public static int GetCurrentUpgradeCost(GameConfig config, GameState state) {
			return GetUpgradeCost(config, state.Upgrade.Level);
		}

		public static int GetNextUpgradeCost(GameConfig config, GameState state) {
			return GetUpgradeCost(config, state.Upgrade.Level + 1);
		}

		public static bool CanUpgrade(GameConfig config, GameState state) {
			return
				HasNextUpgradeLevel(config, state) &&
				(state.Resource.Resources >= GetNextUpgradeCost(config, state));
		}

		public static int GetUpgradeCost(GameConfig config, int upgradeLevel) {
			var levelCount = config.Upgrade.Levels.Length;
			return (upgradeLevel <= levelCount) ? config.Upgrade.Levels[upgradeLevel - 1].Cost : 0;
		}
	}
}
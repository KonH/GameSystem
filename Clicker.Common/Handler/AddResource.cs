using System;
using Clicker.Common.Command;
using Clicker.Common.Config;
using Clicker.Common.State;

namespace Clicker.Common.Handler {
	public static class AddResource {
		public static AddResourceCommand Create(GameConfig config, GameState state, ClickCommand _) {
			var baseValue = config.Resource.ResourceByClick;
			var upgradeLevel = state.Upgrade.Level;
			var upgradePower = (upgradeLevel > 0) ? config.Upgrade.Levels[upgradeLevel - 1].Power : 1.0;
			var amount = (int)Math.Floor(baseValue * upgradePower);
			return new AddResourceCommand { Amount = amount };
		}
	}
}
using Clicker.Common.Command;
using Clicker.Common.Config;
using Clicker.Common.Logic;
using Clicker.Common.State;

namespace Clicker.Common.Handler {
	static class UpgradeHandler {
		public static RemoveResourceCommand Trigger(GameConfig config, GameState state, UpgradeCommand _) {
			var amount = UpgradeLogic.GetCurrentUpgradeCost(config, state);
			return new RemoveResourceCommand { Amount = amount };
		}
	}
}
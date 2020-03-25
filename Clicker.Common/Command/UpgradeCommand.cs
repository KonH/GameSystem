using Clicker.Common.Config;
using Clicker.Common.Logic;
using Clicker.Common.State;
using Core.Common.Command;

namespace Clicker.Common.Command {
	[TrustedCommand]
	public sealed class UpgradeCommand : ICommand<GameConfig, GameState> {
		public CommandResult Apply(GameConfig config, GameState state) {
			if ( !UpgradeLogic.HasNextUpgradeLevel(config, state) ) {
				return CommandResult.BadCommand("No more available upgrade levels");
			}
			state.Upgrade.Level = state.Upgrade.Level + 1;
			return CommandResult.Ok();
		}
	}
}
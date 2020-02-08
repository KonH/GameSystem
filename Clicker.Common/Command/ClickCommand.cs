using Clicker.Common.Config;
using Clicker.Common.State;
using Core.Common.Command;

namespace Clicker.Common.Command {
	public sealed class ClickCommand : ICommand<GameConfig, GameState> {
		public CommandResult Apply(GameConfig config, GameState state) {
			state.Click.Clicks++;
			return CommandResult.Ok();
		}
	}
}
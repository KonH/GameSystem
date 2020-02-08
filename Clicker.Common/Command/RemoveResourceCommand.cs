using Clicker.Common.Config;
using Clicker.Common.State;
using Core.Common.Command;

namespace Clicker.Common.Command {
	public sealed class RemoveResourceCommand : ICommand<GameConfig, GameState> {
		public int Amount { get; set; }

		public CommandResult Apply(GameConfig config, GameState state) {
			if ( Amount <= 0 ) {
				return CommandResult.BadCommand($"Invalid resources amount: {Amount.ToString()}");
			}
			if ( Amount > state.Resource.Resources ) {
				return CommandResult.BadCommand("Not enough resources");
			}
			state.Resource.Resources -= Amount;
			return CommandResult.Ok();
		}

		public override string ToString() {
			return $"{nameof(RemoveResourceCommand)}: {Amount.ToString()}";
		}
	}
}
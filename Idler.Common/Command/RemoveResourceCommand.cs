using Core.Common.Command;
using Idler.Common.Config;
using Idler.Common.State;

namespace Idler.Common.Command {
	public sealed class RemoveResourceCommand : ICommand<GameConfig, GameState> {
		public int Amount { get; set; }

		public RemoveResourceCommand() {}

		public RemoveResourceCommand(int amount) {
			Amount = amount;
		}

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
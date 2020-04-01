using Core.Common.Command;
using Idler.Common.Config;
using Idler.Common.State;

namespace Idler.Common.Command {
	public sealed class AddResourceCommand : ICommand<GameConfig, GameState> {
		public int Amount { get; set; }

		public AddResourceCommand() { }

		public AddResourceCommand(int amount) {
			Amount = amount;
		}

		public CommandResult Apply(GameConfig config, GameState state) {
			if ( Amount <= 0 ) {
				return CommandResult.BadCommand($"Invalid resources amount: {Amount.ToString()}");
			}
			state.Resource.Resources += Amount;
			return CommandResult.Ok();
		}

		public override string ToString() {
			return $"{nameof(AddResourceCommand)}: {Amount.ToString()}";
		}
	}
}
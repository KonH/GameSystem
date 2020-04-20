using Core.Common.Command;
using Idler.Common.Config;
using Idler.Common.State;

namespace Idler.Common.Command {
	[TrustedCommand]
	public sealed class SendSharedResourceCommand : ICommand<GameConfig, GameState> {
		public CommandResult Apply(GameConfig config, GameState state) {
			if ( state.Resource.SharedResources <= 0 ) {
				return CommandResult.BadCommand("Not enough resources");
			}
			return CommandResult.Ok();
		}

		public override string ToString() {
			return $"{nameof(SendSharedResourceCommand)}";
		}
	}
}
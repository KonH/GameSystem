using Core.Common.Command;
using Idler.Common.Config;
using Idler.Common.State;

namespace Idler.Common.Command {
	public sealed class RemoveSharedResourceCommand : ICommand<GameConfig, GameState> {
		public CommandResult Apply(GameConfig config, GameState state) {
			if ( state.Resource.SharedResources <= 0 ) {
				return CommandResult.BadCommand("Not enough resources");
			}
			state.Resource.SharedResources--;
			return CommandResult.Ok();
		}

		public override string ToString() {
			return $"{nameof(RemoveSharedResourceCommand)}";
		}
	}
}
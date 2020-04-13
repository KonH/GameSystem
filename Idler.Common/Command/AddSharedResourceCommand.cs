using Core.Common.Command;
using Idler.Common.Config;
using Idler.Common.State;

namespace Idler.Common.Command {
	[TrustedCommand]
	public sealed class AddSharedResourceCommand : ICommand<GameConfig, GameState> {
		public CommandResult Apply(GameConfig config, GameState state) {
			state.Resource.SharedResources++;
			return CommandResult.Ok();
		}

		public override string ToString() {
			return $"{nameof(AddSharedResourceCommand)}";
		}
	}
}
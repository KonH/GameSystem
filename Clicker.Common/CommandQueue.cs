using Clicker.Common.Command;
using Clicker.Common.Config;
using Clicker.Common.Handler;
using Clicker.Common.State;
using Core.Common.CommandDependency;

namespace Clicker.Common {
	public sealed class CommandQueue : CommandQueue<GameConfig, GameState> {
		public CommandQueue() {
			AddDependency<ClickCommand, AddResourceCommand>(ClickHandler.Trigger);
			AddDependency<UpgradeCommand, RemoveResourceCommand>(UpgradeHandler.Trigger);
		}
	}
}
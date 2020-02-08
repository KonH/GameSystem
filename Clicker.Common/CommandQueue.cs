using Clicker.Common.Command;
using Clicker.Common.Config;
using Clicker.Common.Handler;
using Clicker.Common.State;
using Core.Common.CommandDependency;

namespace Clicker.Common {
	public sealed class CommandQueue {
		public readonly CommandQueue<GameConfig, GameState> Queue = new CommandQueue<GameConfig, GameState>();

		public CommandQueue() {
			Queue.AddDependency<ClickCommand, AddResourceCommand>(AddResource.Create);
			Queue.AddDependency<UpgradeCommand, RemoveResourceCommand>(RemoveResource.Create);
		}
	}
}
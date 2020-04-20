using Core.Common.CommandDependency;
using Idler.Common.Command;
using Idler.Common.Config;
using Idler.Common.Handler;
using Idler.Common.State;

namespace Idler.Common {
	public sealed class CommandQueue : CommandQueue<GameConfig, GameState> {
		public CommandQueue() {
			AddDependency<AddSharedResourceCommand, RemoveResourceCommand>(SharedResourceHandler.Trigger);
			AddDependency<SendSharedResourceCommand, RemoveSharedResourceCommand>(c => new RemoveSharedResourceCommand());
		}
	}
}
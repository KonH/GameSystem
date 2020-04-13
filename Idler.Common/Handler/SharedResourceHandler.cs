using Idler.Common.Command;
using Idler.Common.Config;
using Idler.Common.State;

namespace Idler.Common.Handler {
	public static class SharedResourceHandler {
		public static RemoveResourceCommand Trigger(GameConfig config, GameState state, AddSharedResourceCommand _) {
			return new RemoveResourceCommand(config.Resource.SharedCost);
		}
	}
}
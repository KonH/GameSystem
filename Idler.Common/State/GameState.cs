using Core.Common.State;

namespace Idler.Common.State {
	public sealed class GameState : IState {
		public StateVersion  Version  { get; set; } = new StateVersion();
		public ResourceState Resource { get; set; } = new ResourceState();
	}
}
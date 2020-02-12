using Core.Common.State;

namespace Clicker.Common.State {
	public sealed class GameState : IState {
		public StateVersion  Version  { get; set; } = new StateVersion();
		public ClickState    Click    { get; set; } = new ClickState();
		public ResourceState Resource { get; set; } = new ResourceState();
		public UpgradeState  Upgrade  { get; set; } = new UpgradeState();
	}
}
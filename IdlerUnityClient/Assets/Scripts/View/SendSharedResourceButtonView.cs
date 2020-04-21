using Core.Client.UnityClient.Component.View;
using Idler.Common.Config;
using Idler.Common.State;

namespace Idler.UnityClient.View {
	public sealed class SendSharedResourceButtonView : ButtonView {
		public void Init(GameConfig config, GameState state) {
			base.Init();
			UpdateState(config, state);
		}

		public void UpdateState(GameConfig config, GameState state) {
			var isActive = state.Resource.SharedResources > 0;
			UpdateState(isActive);
		}
	}
}
using Clicker.Common.Config;
using Clicker.Common.State;
using Core.Client.UnityClient.Component.View;
using UnityEngine;

namespace Clicker.UnityClient.View {
	// TODO
	public sealed class UpgradeButtonView : ButtonView {
		[SerializeField] TextView _text = null;

		public void Init(GameConfig config, GameState state) {
			base.Init();
			_text.Init(1);
			UpdateState(config, state);
		}

		public void UpdateState(GameConfig config, GameState state) {
			UpdateState(true);
		}
	}
}
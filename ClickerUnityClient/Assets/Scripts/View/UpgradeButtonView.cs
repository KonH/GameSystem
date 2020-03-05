using Clicker.Common.Config;
using Clicker.Common.Logic;
using Clicker.Common.State;
using Core.Client.UnityClient.Component.View;
using UnityEngine;

namespace Clicker.UnityClient.View {
	public sealed class UpgradeButtonView : ButtonView {
		[SerializeField] TextView _text = null;

		public void Init(GameConfig config, GameState state) {
			base.Init();
			_text.Init();
			UpdateState(config, state);
		}

		public void UpdateState(GameConfig config, GameState state) {
			if ( !UpgradeLogic.HasNextUpgradeLevel(config, state) ) {
				gameObject.SetActive(false);
				return;
			}
			var cost = UpgradeLogic.GetNextUpgradeCost(config, state);
			_text.Init(cost);
			var isActive = UpgradeLogic.CanUpgrade(config, state);
			UpdateState(isActive);
		}
	}
}
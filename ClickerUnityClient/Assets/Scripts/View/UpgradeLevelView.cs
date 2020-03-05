using System.Threading.Tasks;
using Clicker.Common.Config;
using Clicker.Common.State;
using Core.Client.UnityClient.Component.View;
using UnityEngine;

namespace Clicker.UnityClient.View {
	public sealed class UpgradeLevelView : AnimatedTextView {
		[SerializeField] AnimatedTextView  _appearText    = null;
		[SerializeField] UpgradeButtonView _upgradeButton = null;

		public void Init(GameConfig config, GameState state) {
			base.Init();
			_appearText.Init();
			_upgradeButton.Init();
			UpdateValue(config, state);
		}

		public async Task AnimateValue(GameConfig config, GameState state) {
			await _appearText.AnimateValue(1);
			await base.AnimateValue(state.Upgrade.Level);
		}

		void UpdateValue(GameConfig config, GameState state) {
			UpdateValue(state.Upgrade.Level);
		}
	}
}
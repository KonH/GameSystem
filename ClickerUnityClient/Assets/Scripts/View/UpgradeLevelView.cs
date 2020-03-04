using System.Threading.Tasks;
using Clicker.Common.Config;
using Clicker.Common.State;
using Core.Client.UnityClient.Component.View;
using UnityEngine;

namespace Clicker.UnityClient.View {
	// TODO
	public sealed class UpgradeLevelView : AnimatedTextView {
		[SerializeField] AnimatedTextView  _appearText    = null;
		[SerializeField] UpgradeButtonView _upgradeButton = null;

		public void Init(GameConfig config, GameState state) {
			base.Init();
			_appearText.Init(0);
			_upgradeButton.Init(true);
			UpdateValue(config, state);
		}

		public Task AnimateValue(GameConfig config, GameState state) {
			return Task.CompletedTask;
		}

		void UpdateValue(GameConfig config, GameState state) {
			UpdateValue(state.Upgrade.Level);
		}
	}
}
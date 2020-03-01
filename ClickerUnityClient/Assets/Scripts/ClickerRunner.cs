using Clicker.Common.Command;
using Clicker.Common.Config;
using Clicker.Common.State;
using Core.Client.Shared;
using Core.Client.UnityClient;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Clicker.UnityClient {
	public sealed class ClickerRunner : UnityRunner<GameConfig, GameState> {
		[SerializeField] GameObject _stateView        = null;
		[SerializeField] TMP_Text   _resourcesText    = null;
		[SerializeField] TMP_Text   _upgradeLevelText = null;
		[SerializeField] Button     _clickButton      = null;
		[SerializeField] Button     _upgradeButton    = null;

		void OnValidate() {
			Debug.Assert(_stateView);
			Debug.Assert(_resourcesText);
			Debug.Assert(_upgradeLevelText);
			Debug.Assert(_clickButton);
			Debug.Assert(_upgradeButton);
		}

		async void Awake() {
			_clickButton.onClick.AddListener(HandleClick);
			_upgradeButton.onClick.AddListener(HandleUpgrade);
			DisableStateView();
			await Initialize();
		}

		protected override void HandleInitialization(InitializationResult result) {
			switch ( result ) {
				case InitializationResult.Ok _: {
					IsReadyForCommand = true;
					EnableStateView();
					UpdateState();
					break;
				}

				case InitializationResult.Error error: {
					Debug.LogError(error.Description);
					break;
				}
			}
		}

		protected override void HandleCommandResult(CommandApplyResult result) {
			switch ( result ) {
				case CommandApplyResult.Ok _: {
					UpdateState();
					break;
				}

				case CommandApplyResult.Error error: {
					Debug.LogError(error.Description);
					break;
				}
			}
		}

		void DisableStateView() {
			_stateView.SetActive(false);
		}

		void EnableStateView() {
			_stateView.SetActive(true);
		}

		void UpdateState() {
			_resourcesText.text    = State.Resource.Resources.ToString();
			_upgradeLevelText.text = State.Upgrade.Level.ToString();
		}

		void HandleClick() {
			EnqueueCommand(new ClickCommand());
		}

		void HandleUpgrade() {
			EnqueueCommand(new UpgradeCommand());
		}
	}
}
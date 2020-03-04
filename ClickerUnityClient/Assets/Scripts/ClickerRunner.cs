using Clicker.Common.Command;
using Clicker.Common.Config;
using Clicker.Common.State;
using Clicker.UnityClient.Handler;
using Core.Client.Shared;
using Core.Client.UnityClient;
using Core.Client.UnityClient.DependencyInjection;
using Core.Common.CommandExecution;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Clicker.UnityClient {
	public sealed class ClickerRunner : UnityRunner<GameConfig, GameState> {
		[SerializeField] GameObject _stateView = null;

		[Header("Resources")]
		[SerializeField] TMP_Text _resourcesText    = null;
		[SerializeField] TMP_Text _addResourcesText = null;

		[Header("Upgrades")]
		[SerializeField] TMP_Text _upgradeLevelText = null;
		[SerializeField] TMP_Text _addUpgradeText   = null;

		[Header("Controls")]
		[SerializeField] Button _clickButton   = null;
		[SerializeField] Button _upgradeButton = null;

		void OnValidate() {
			Debug.Assert(_stateView, nameof(_stateView));
			Debug.Assert(_resourcesText, nameof(_resourcesText));
			Debug.Assert(_addResourcesText, nameof(_addResourcesText));
			Debug.Assert(_upgradeLevelText, nameof(_upgradeLevelText));
			Debug.Assert(_addUpgradeText, nameof(_addUpgradeText));
			Debug.Assert(_clickButton, nameof(_clickButton));
			Debug.Assert(_upgradeButton, nameof(_upgradeButton));
		}

		async void Awake() {
			AddHandlers();
			_clickButton.onClick.AddListener(HandleClick);
			_upgradeButton.onClick.AddListener(HandleUpgrade);
			DisableStateView();
			await Initialize();
		}

		void AddHandlers() {
			var serviceProvider = ServiceProvider.Instance;
			var executor        = serviceProvider.GetService<CommandExecutor<GameConfig, GameState>>();
			executor.AddHandler(new AddResourceCommandHandler(_resourcesText, _addResourcesText));
			executor.AddHandler(new UpgradeCommandHandler(_upgradeLevelText, _addUpgradeText));
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
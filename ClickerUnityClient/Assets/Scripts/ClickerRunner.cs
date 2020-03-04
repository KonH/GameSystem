using Clicker.Common.Command;
using Clicker.Common.Config;
using Clicker.Common.State;
using Clicker.UnityClient.Handler;
using Clicker.UnityClient.View;
using Core.Client.Shared;
using Core.Client.UnityClient;
using Core.Client.UnityClient.DependencyInjection;
using Core.Common.CommandExecution;
using UnityEngine;
using UnityEngine.UI;

namespace Clicker.UnityClient {
	public sealed class ClickerRunner : UnityRunner<GameConfig, GameState> {
		[SerializeField] GameObject _stateView = null;

		[Header("Resources")]
		[SerializeField] ResourceView _resourceView = null;

		[Header("Upgrades")]
		[SerializeField] UpgradeLevelView _upgradeLevelView = null;

		[Header("Controls")]
		[SerializeField] Button _clickButton   = null;
		[SerializeField] Button _upgradeButton = null;

		void OnValidate() {
			Debug.Assert(_stateView, nameof(_stateView));
			Debug.Assert(_resourceView, nameof(_resourceView));
			Debug.Assert(_upgradeLevelView, nameof(_upgradeLevelView));
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
			executor.AddHandler(new AddResourceCommandHandler(_resourceView));
			executor.AddHandler(new RemoveResourceCommandHandler(_resourceView));
			executor.AddHandler(new UpgradeCommandHandler(_upgradeLevelView));
		}

		protected override void HandleInitialization(InitializationResult result) {
			switch ( result ) {
				case InitializationResult.Ok _: {
					IsReadyForCommand = true;
					EnableStateView();
					InitViews();
					break;
				}

				case InitializationResult.Error error: {
					Debug.LogError(error.Description);
					break;
				}
			}
		}

		protected override void HandleCommandResult(CommandApplyResult result) {
			if ( result is CommandApplyResult.Error error ) {
				Debug.LogError(error.Description);
			}
		}

		void DisableStateView() {
			_stateView.SetActive(false);
		}

		void EnableStateView() {
			_stateView.SetActive(true);
		}

		void InitViews() {
			_resourceView.Init(State);
			_upgradeLevelView.Init(null, State);
		}

		void HandleClick() {
			EnqueueCommand(new ClickCommand());
		}

		void HandleUpgrade() {
			EnqueueCommand(new UpgradeCommand());
		}
	}
}
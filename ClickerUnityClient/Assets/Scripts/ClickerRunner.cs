using System.Threading;
using System.Threading.Tasks;
using Clicker.Common.Command;
using Clicker.Common.Config;
using Clicker.Common.State;
using Clicker.UnityClient.Reaction;
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
		[SerializeField] UpgradeButtonView _upgradeButtonView = null;
		[SerializeField] UpgradeLevelView  _upgradeLevelView  = null;

		[Header("Controls")]
		[SerializeField] Button _clickButton   = null;
		[SerializeField] Button _upgradeButton = null;

		void OnValidate() {
			Debug.Assert(_stateView, nameof(_stateView));
			Debug.Assert(_resourceView, nameof(_resourceView));
			Debug.Assert(_upgradeButtonView, nameof(_upgradeButtonView));
			Debug.Assert(_upgradeLevelView, nameof(_upgradeLevelView));
			Debug.Assert(_clickButton, nameof(_clickButton));
			Debug.Assert(_upgradeButton, nameof(_upgradeButton));
		}

		async void Awake() {
			AddReactions();
			_clickButton.onClick.AddListener(HandleClick);
			_upgradeButton.onClick.AddListener(HandleUpgrade);
			DisableStateView();
			await Initialize();
		}

		void AddReactions() {
			var serviceProvider = ServiceProvider.Instance;
			var executor        = serviceProvider.GetService<CommandExecutor<GameConfig, GameState>>();
			executor.AddReaction(new AddResourceCommandReaction(_resourceView, _upgradeButtonView));
			executor.AddReaction(new RemoveResourceCommandReaction(_resourceView, _upgradeButtonView));
			executor.AddReaction(new UpgradeCommandReaction(_upgradeLevelView));
		}

		protected override Task HandleInitialization(InitializationResult result, CancellationToken cancellationToken) {
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
			return Task.CompletedTask;
		}

		protected override Task HandleCommandResult(CommandApplyResult result, CancellationToken cancellationToken) {
			if ( result is CommandApplyResult.Error error ) {
				Debug.LogError(error.Description);
			}
			return Task.CompletedTask;
		}

		void DisableStateView() {
			_stateView.SetActive(false);
		}

		void EnableStateView() {
			_stateView.SetActive(true);
		}

		void InitViews() {
			_resourceView.Init(State);
			_upgradeLevelView.Init(Config, State);
			_upgradeButtonView.Init(Config, State);
		}

		void HandleClick() {
			EnqueueCommand(new ClickCommand());
		}

		void HandleUpgrade() {
			EnqueueCommand(new UpgradeCommand());
		}
	}
}
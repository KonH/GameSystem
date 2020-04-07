using Idler.Common.Config;
using Idler.Common.State;
using Idler.UnityClient.View;
using Core.Client.Shared;
using Core.Client.UnityClient;
using Core.Client.UnityClient.DependencyInjection;
using Core.Common.CommandExecution;
using Idler.UnityClient.Reaction;
using UnityEngine;

namespace Idler.UnityClient {
	public sealed class IdlerRunner : UnityRunner<GameConfig, GameState> {
		[SerializeField] GameObject _stateView = null;

		[Header("Resources")]
		[SerializeField] ResourceView _resourceView = null;

		void OnValidate() {
			Debug.Assert(_stateView, nameof(_stateView));
			Debug.Assert(_resourceView, nameof(_resourceView));
		}

		async void Awake() {
			AddReactions();
			DisableStateView();
			await Initialize();
		}

		void AddReactions() {
			var serviceProvider = ServiceProvider.Instance;
			var executor        = serviceProvider.GetService<CommandExecutor<GameConfig, GameState>>();
			executor.AddReaction(new AddResourceCommandReaction(_resourceView));
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
		}
	}
}
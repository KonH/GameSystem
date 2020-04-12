using System.Threading.Tasks;
using Idler.Common.Config;
using Idler.Common.State;
using Idler.UnityClient.View;
using Core.Client.Shared;
using Core.Client.UnityClient;
using Core.Client.UnityClient.DependencyInjection;
using Core.Client.UnityClient.Window;
using Core.Common.CommandExecution;
using Idler.UnityClient.Reaction;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Idler.UnityClient {
	public sealed class IdlerRunner : UnityRunner<GameConfig, GameState> {
		[SerializeField] GameObject _stateView = null;

		[Header("Resources")]
		[SerializeField] ResourceView _resourceView = null;

		[Header("Windows")]
		[SerializeField] WindowSettings _windowSettings = null;

		WindowManager _windowManager = null;

		void OnValidate() {
			Debug.Assert(_stateView, nameof(_stateView));
			Debug.Assert(_resourceView, nameof(_resourceView));
			Debug.Assert(_windowSettings, nameof(_windowSettings));
		}

		async void Awake() {
			_windowManager = new WindowManager(_windowSettings);
			AddReactions();
			DisableStateView();
			await Initialize();
		}

		void AddReactions() {
			var serviceProvider = ServiceProvider.Instance;
			var executor        = serviceProvider.GetService<CommandExecutor<GameConfig, GameState>>();
			executor.AddReaction(new AddResourceCommandReaction(_resourceView));
		}

		protected override async Task HandleInitialization(InitializationResult result) {
			switch ( result ) {
				case InitializationResult.Ok _: {
					IsReadyForCommand = true;
					EnableStateView();
					InitViews();
					break;
				}

				case InitializationResult.Error error: {
					Debug.LogError(error.Description);
					await _windowManager.Show<MessageWindow>(
						nameof(MessageWindow),
						w => w.Show("Server Error", "Failed to connect to server", "Retry"));
					SceneManager.LoadScene(SceneManager.GetActiveScene().name);
					break;
				}
			}
		}

		protected override async Task HandleCommandResult(CommandApplyResult result) {
			if ( result is CommandApplyResult.Error error ) {
				Debug.LogError(error.Description);
				await _windowManager.Show<MessageWindow>(
					nameof(MessageWindow),
					w => w.Show("Command Error", "Error happens while communication with server", "Retry"));
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
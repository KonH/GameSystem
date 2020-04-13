using System.Threading.Tasks;
using Idler.Common.Config;
using Idler.Common.State;
using Idler.UnityClient.View;
using Core.Client.Shared;
using Core.Client.UnityClient;
using Core.Client.UnityClient.DependencyInjection;
using Core.Client.UnityClient.Window;
using Core.Common.CommandExecution;
using Core.Service.Shared;
using Idler.Common.Command;
using Idler.UnityClient.Reaction;
using Idler.UnityClient.UpdateHandler;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Idler.UnityClient {
	public sealed class IdlerRunner : UnityRunner<GameConfig, GameState> {
		[SerializeField] GameObject _stateView = null;

		[Header("Resources")]
		[SerializeField] ResourceView                _resourceView                = null;
		[SerializeField] SharedResourceView          _sharedResourceView          = null;
		[SerializeField] Slider                      _incomeSlider                = null;
		[SerializeField] AddSharedResourceButtonView _addSharedResourceButtonView = null;

		[Header("Controls")]
		[SerializeField] Button _addSharedResourceButton = null;


		[Header("Windows")]
		[SerializeField] WindowSettings _windowSettings = null;

		WindowManager _windowManager = null;

		void OnValidate() {
			Debug.Assert(_stateView, nameof(_stateView));
			Debug.Assert(_resourceView, nameof(_resourceView));
			Debug.Assert(_incomeSlider, nameof(_incomeSlider));
			Debug.Assert(_addSharedResourceButtonView, nameof(_addSharedResourceButtonView));
			Debug.Assert(_addSharedResourceButton, nameof(_addSharedResourceButton));
			Debug.Assert(_windowSettings, nameof(_windowSettings));
		}

		async void Awake() {
			_windowManager = new WindowManager(_windowSettings);
			_addSharedResourceButton.onClick.AddListener(HandleAddSharedResource);
			AddReactions();
			AddHandlers();
			DisableStateView();
			await Initialize();
		}

		void AddReactions() {
			var serviceProvider = ServiceProvider.Instance;
			var executor        = serviceProvider.GetService<CommandExecutor<GameConfig, GameState>>();
			executor.AddReaction(new AddResourceCommandReaction(_resourceView, _addSharedResourceButtonView));
			executor.AddReaction(new RemoveResourceCommandReaction(_resourceView, _addSharedResourceButtonView));
			executor.AddReaction(new AddSharedResourceCommandReaction(_sharedResourceView));
		}

		void AddHandlers() {
			var serviceProvider = ServiceProvider.Instance;
			var time            = serviceProvider.GetService<ITimeProvider>();
			AddUpdateHandler(new IncomeHandler(_incomeSlider, time));
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
			_sharedResourceView.Init(State);
			_addSharedResourceButtonView.Init(Config, State);
		}

		void HandleAddSharedResource() {
			EnqueueCommand(new AddSharedResourceCommand());
		}
	}
}
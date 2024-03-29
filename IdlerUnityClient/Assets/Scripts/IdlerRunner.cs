using System.Threading;
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
using Idler.UnityClient.Repository;
using Idler.UnityClient.UpdateHandler;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Idler.UnityClient {
	public sealed class IdlerRunner : UnityRunner<GameConfig, GameState> {
		[SerializeField] GameObject _stateView = null;

		[Header("Resources")]
		[SerializeField] ResourceView                 _resourceView                 = null;
		[SerializeField] SharedResourceView           _sharedResourceView           = null;
		[SerializeField] Slider                       _incomeSlider                 = null;
		[SerializeField] AddSharedResourceButtonView  _addSharedResourceButtonView  = null;
		[SerializeField] SendSharedResourceButtonView _sendSharedResourceButtonView = null;
		[SerializeField] SharedResourceRemoteView     _sharedResourceRemoteView     = null;

		[Header("Controls")]
		[SerializeField] Button _addSharedResourceButton  = null;
		[SerializeField] Button _sendSharedResourceButton = null;


		[Header("Windows")]
		[SerializeField] WindowSettings _windowSettings = null;

		WindowManager _windowManager = null;

		void OnValidate() {
			Debug.Assert(_stateView, nameof(_stateView));
			Debug.Assert(_resourceView, nameof(_resourceView));
			Debug.Assert(_incomeSlider, nameof(_incomeSlider));
			Debug.Assert(_addSharedResourceButtonView, nameof(_addSharedResourceButtonView));
			Debug.Assert(_sharedResourceRemoteView, nameof(_sharedResourceRemoteView));
			Debug.Assert(_addSharedResourceButton, nameof(_addSharedResourceButton));
			Debug.Assert(_sendSharedResourceButton, nameof(_sendSharedResourceButton));
			Debug.Assert(_windowSettings, nameof(_windowSettings));
		}

		async void Awake() {
			IdlerEntryPoint.Install();
			_windowManager = new WindowManager(_windowSettings);
			_addSharedResourceButton.onClick.AddListener(HandleAddSharedResource);
			_sendSharedResourceButton.onClick.AddListener(HandleSendSharedResource);
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
			executor.AddReaction(new AddSharedResourceCommandReaction(_sharedResourceView, _sendSharedResourceButtonView));
			executor.AddReaction(new SendSharedResourceCommandReaction(_sharedResourceRemoteView));
			executor.AddReaction(new RemoveSharedResourceCommandReaction(_sharedResourceView, _sendSharedResourceButtonView));
		}

		void AddHandlers() {
			var serviceProvider = ServiceProvider.Instance;
			var time            = serviceProvider.GetService<ITimeProvider>();
			AddUpdateHandler(new IncomeHandler(_incomeSlider, time));
		}

		protected override async Task HandleInitialization(InitializationResult result, CancellationToken cancellationToken) {
			switch ( result ) {
				case InitializationResult.Ok _: {
					IsReadyForCommand = true;
					EnableStateView();
					await InitViews();
					break;
				}

				case InitializationResult.Error error: {
					Debug.LogError(error.Description);
					await _windowManager.Show<MessageWindow>(
						nameof(MessageWindow),
						(w, ct) => w.Show("Server Error", "Failed to connect to server", "Retry", ct),
						cancellationToken);
					SceneManager.LoadScene(SceneManager.GetActiveScene().name);
					break;
				}
			}
		}

		protected override async Task HandleCommandResult(CommandApplyResult result, CancellationToken cancellationToken) {
			if ( result is CommandApplyResult.Error error ) {
				Debug.LogError(error.Description);
				await _windowManager.Show<MessageWindow>(
					nameof(MessageWindow),
					(w, ct) => w.Show("Command Error", "Error happens while communication with server", "Retry", ct),
					cancellationToken);
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			}
		}

		void DisableStateView() {
			_stateView.SetActive(false);
		}

		void EnableStateView() {
			_stateView.SetActive(true);
		}

		async Task InitViews() {
			_resourceView.Init(State);
			_sharedResourceView.Init(State);
			_addSharedResourceButtonView.Init(Config, State);
			_sendSharedResourceButtonView.Init(Config, State);
			await _sharedResourceRemoteView.Init(ServiceProvider.Instance.GetService<ISharedStateProvider>());
		}

		void HandleAddSharedResource() {
			EnqueueCommand(new AddSharedResourceCommand());
		}

		void HandleSendSharedResource() {
			EnqueueCommand(new SendSharedResourceCommand());
		}
	}
}
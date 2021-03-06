using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Client.Abstractions;
using Core.Client.Shared;
using Core.Client.UnityClient.DependencyInjection;
using Core.Client.UnityClient.Threading;
using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;
using UnityEngine;

namespace Core.Client.UnityClient {
	public abstract class UnityRunner<TConfig, TState> : MonoBehaviour where TConfig : IConfig where TState : IState {
		public TState State => _client.State;
		public TConfig Config => _client.Config;

		public bool IsReadyForCommand { get; protected set; }

		IClient<TConfig, TState> _client;

		List<IUpdateHandler<TConfig, TState>> _updateHandlers    = new List<IUpdateHandler<TConfig, TState>>();
		Queue<ICommand<TConfig, TState>>      _commands          = new Queue<ICommand<TConfig, TState>>();
		CancellationTokenSource               _cancelTokenSource = new CancellationTokenSource();

		protected void AddUpdateHandler(IUpdateHandler<TConfig, TState> handler) {
			_updateHandlers.Add(handler);
		}

		protected async Task Initialize() {
			_client = ServiceProvider.Instance.GetService<IClient<TConfig, TState>>();
			await Async.WaitForBackgroundThread;
			var result = await _client.Initialize(_cancelTokenSource.Token);
			await Async.WaitForUpdate;
			await HandleInitialization(result, _cancelTokenSource.Token);
		}

		protected void OnDestroy() {
			_cancelTokenSource.Cancel();
		}

		protected void EnqueueCommand(ICommand<TConfig, TState> command) {
			if ( !IsReadyForCommand ) {
				Debug.LogWarning("Not yet ready");
				return;
			}
			_commands.Enqueue(command);
		}

		async void Update() {
			if ( (Config != null) && (State != null) ) {
				foreach ( var handler in _updateHandlers ) {
					handler.Update(Config, State);
				}
			}
			var hasAnyCommand = (_commands.Count > 0);
			if ( !hasAnyCommand ) {
				return;
			}
			var command = _commands.Dequeue();
			await Async.WaitForBackgroundThread;
			await HandleCommand(command, _cancelTokenSource.Token);
		}

		protected abstract Task HandleInitialization(InitializationResult result, CancellationToken cancellationToken);

		async Task HandleCommand(ICommand<TConfig, TState> command, CancellationToken cancellationToken) {
			var result = await _client.Apply(command, cancellationToken);
			await Async.WaitForUpdate;
			await HandleCommandResult(result, cancellationToken);
		}

		protected abstract Task HandleCommandResult(CommandApplyResult result, CancellationToken cancellationToken);
	}
}
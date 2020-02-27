using System.Collections.Generic;
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

		public bool IsReadyForCommand { get; protected set; }

		IClient<TConfig, TState> _client;

		Queue<ICommand<TConfig, TState>> _commands = new Queue<ICommand<TConfig, TState>>();

		protected async Task Initialize() {
			_client = ServiceProvider.Instance.GetService<IClient<TConfig, TState>>();
			await Async.WaitForBackgroundThread;
			var result = await _client.Initialize();
			await Async.WaitForUpdate;
			HandleInitialization(result);
		}

		protected void EnqueueCommand(ICommand<TConfig, TState> command) {
			if ( !IsReadyForCommand ) {
				Debug.LogWarning("Not yet ready");
				return;
			}
			_commands.Enqueue(command);
		}

		async void Update() {
			var hasAnyCommand = (_commands.Count > 0);
			if ( !hasAnyCommand ) {
				return;
			}
			var command = _commands.Dequeue();
			await Async.WaitForBackgroundThread;
			await HandleCommand(command);
		}

		protected abstract void HandleInitialization(InitializationResult result);

		async Task HandleCommand(ICommand<TConfig, TState> command) {
			var result = await _client.Apply(command);
			await Async.WaitForUpdate;
			HandleCommandResult(result);
		}

		protected abstract void HandleCommandResult(CommandApplyResult result);
	}
}
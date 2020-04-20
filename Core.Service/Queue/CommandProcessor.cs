using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;
using Core.Common.Threading;
using Core.Service.Model;

namespace Core.Service.Queue {
	/// <summary>
	/// React to commands on service-side
	/// </summary>
	public sealed class CommandProcessor<TConfig, TState> where TConfig : IConfig where TState : IState {
		readonly ITaskRunner _taskRunner;

		readonly Dictionary<Type, List<Func<UserId, ICommand<TConfig, TState>, Task>>> _handlers =
			new Dictionary<Type, List<Func<UserId, ICommand<TConfig, TState>, Task>>>();

		public CommandProcessor(ITaskRunner taskRunner) {
			_taskRunner = taskRunner;
		}

		public void Handle<T>(Func<UserId, T, Task> handler) where T : ICommand<TConfig, TState> {
			Task StoredHandler(UserId userId, ICommand<TConfig, TState> c) => handler(userId, (T)c);
			if ( _handlers.TryGetValue(typeof(T), out var handlers) ) {
				handlers.Add(StoredHandler);
			} else {
				handlers = new List<Func<UserId, ICommand<TConfig, TState>, Task>> {
					StoredHandler
				};
				_handlers.Add(typeof(T), handlers);
			}
		}

		public void OnNewCommand(UserId userId, ICommand<TConfig, TState> command) {
			if ( _handlers.TryGetValue(command.GetType(), out var handlers) ) {
				_taskRunner.Run(async _ => {
					foreach ( var handler in handlers ) {
						await handler(userId, command);
					}
				}, CancellationToken.None);
			}
		}
	}
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Common.Command;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;
using Core.Service.Extension;
using Core.Service.Repository.State;

namespace Core.Service.Queue {
	public sealed class CommandScheduler<TConfig, TState> where TConfig : IConfig where TState : IState {
		public sealed class Settings {
			public IReadOnlyCollection<IUpdateWatcher<TConfig, TState>> Watchers => _watchers.AsReadOnly();

			List<IUpdateWatcher<TConfig, TState>> _watchers = new List<IUpdateWatcher<TConfig, TState>>();

			public void AddWatcher(IUpdateWatcher<TConfig, TState> watcher) {
				_watchers.Add(watcher);
			}
		}

		static readonly CommandSet<TConfig, TState> Commands = new CommandSet<TConfig, TState>();

		readonly Settings                              _settings;
		readonly CommandWorkQueue<TConfig, TState>     _queue;
		readonly IStateRepository<TState>              _stateRepository;
		readonly BatchCommandExecutor<TConfig, TState> _executor;

		public CommandScheduler(
			Settings                 settings,        CommandWorkQueue<TConfig, TState>     queue,
			IStateRepository<TState> stateRepository, BatchCommandExecutor<TConfig, TState> executor) {
			_settings        = settings;
			_queue           = queue;
			_stateRepository = stateRepository;
			_executor        = executor;
		}

		public async Task Update() {
			await _queue.ForEachPending(async (userId, item) => {
				foreach ( var watcher in _settings.Watchers ) {
					watcher.TryAddCommands(userId, item.Config, item.State, Commands);
				}
				if ( !Commands.IsEmpty ) {
					var commands = Commands.ToArray();
					var finalCommands = new List<ICommand<TConfig, TState>>();
					var finalErrors = new List<BatchCommandResult>();
					foreach ( var command in commands ) {
						var state = await _stateRepository.Get(userId);
						var result = await _executor.Apply(item.Config, state, command);
						if ( result is BatchCommandResult.Ok<TConfig, TState> ok ) {
							finalCommands.Add(command);
							finalCommands.AddRange(ok.NextCommands);
							await _stateRepository.Update(userId, state);
						} else {
							finalErrors.Add(result);
						}
					}
					item.Complete(finalCommands.ToArray(), finalErrors.ToArray());
				}
			});
		}
	}
}
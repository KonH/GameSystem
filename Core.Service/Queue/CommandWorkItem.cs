using System.Threading.Tasks;
using Core.Common.Command;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Service.Queue {
	public sealed class CommandWorkItem<TConfig, TState> where TConfig : IConfig where TState : IState {
		public readonly TConfig Config;
		public readonly TState  State;

		public Task<CommandQueueResult<TConfig, TState>> Task        => _tcs.Task;
		public bool                                      IsCompleted => _tcs.Task.IsCompleted;

		readonly TaskCompletionSource<CommandQueueResult<TConfig, TState>> _tcs =
			new TaskCompletionSource<CommandQueueResult<TConfig, TState>>();

		public CommandWorkItem(TConfig config, TState state) {
			Config = config;
			State  = state;
		}

		internal void Complete(ICommand<TConfig, TState>[] commands, BatchCommandResult[] errors) {
			var result = new CommandQueueResult<TConfig, TState>(commands, errors);
			_tcs.SetResult(result);
		}
	}
}
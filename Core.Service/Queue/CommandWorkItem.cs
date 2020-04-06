using System.Threading.Tasks;
using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Service.Queue {
	public sealed class CommandWorkItem<TConfig, TState> where TConfig : IConfig where TState : IState {
		public readonly TConfig Config;
		public readonly TState  State;

		public Task<ICommand<TConfig, TState>[]> Task        => _tcs.Task;
		public bool                              IsCompleted => _tcs.Task.IsCompleted;

		readonly TaskCompletionSource<ICommand<TConfig, TState>[]> _tcs = new TaskCompletionSource<ICommand<TConfig, TState>[]>();

		public CommandWorkItem(TConfig config, TState state) {
			Config = config;
			State  = state;
		}

		internal void Complete(ICommand<TConfig, TState>[] result) {
			_tcs.SetResult(result);
		}
	}
}
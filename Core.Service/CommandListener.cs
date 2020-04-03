using System.Threading.Tasks;
using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Service {
	sealed class CommandListener<TConfig, TState> where TConfig : IConfig where TState : IState {
		public readonly TConfig Config;
		public readonly TState  State;

		public readonly TaskCompletionSource<ICommand<TConfig, TState>[]> CompletionSource = new TaskCompletionSource<ICommand<TConfig, TState>[]>();

		public CommandListener(TConfig config, TState state) {
			Config = config;
			State  = state;
		}
	}
}
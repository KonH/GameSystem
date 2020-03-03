using System.Threading.Tasks;
using Core.Client.UnityClient.Threading;
using Core.Common.Command;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Client.UnityClient.CommandExecution {
	public abstract class UnityCommandHandler<TConfig, TState, TCommand> :
		ICommandHandler<TConfig, TState, TCommand>
		where TConfig : IConfig
		where TState : IState
		where TCommand : ICommand<TConfig, TState> {

		public async Task Before(TState state, TCommand command) {
			await Async.WaitForUpdate;
			await BeforeOnMainThread(state, command);
		}

		public abstract Task BeforeOnMainThread(TState state, TCommand command);

		public async Task After(TState state, TCommand command) {
			await Async.WaitForUpdate;
			await AfterOnMainThread(state, command);
		}

		public abstract Task AfterOnMainThread(TState state, TCommand command);
	}
}
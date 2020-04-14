using System.Threading;
using System.Threading.Tasks;
using Core.Client.UnityClient.Threading;
using Core.Common.Command;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Client.UnityClient.CommandExecution {
	public abstract class UnityCommandReaction<TConfig, TState, TCommand> :
		ICommandReaction<TConfig, TState, TCommand>
		where TConfig : IConfig
		where TState : IState
		where TCommand : ICommand<TConfig, TState> {

		public async Task Before(TConfig config, TState state, TCommand command, CancellationToken cancellationToken) {
			await Async.WaitForUpdate;
			await BeforeOnMainThread(config, state, command, cancellationToken);
		}

		public virtual Task BeforeOnMainThread(TConfig config, TState state, TCommand command, CancellationToken cancellationToken) {
			return Task.CompletedTask;
		}

		public async Task After(TConfig config, TState state, TCommand command, CancellationToken cancellationToken) {
			await Async.WaitForUpdate;
			await AfterOnMainThread(config, state, command, cancellationToken);
		}

		public virtual Task AfterOnMainThread(TConfig config, TState state, TCommand command, CancellationToken cancellationToken) {
			return Task.CompletedTask;
		}
	}
}
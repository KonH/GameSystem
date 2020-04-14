using System.Threading;
using System.Threading.Tasks;
using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Common.CommandExecution {
	public interface ICommandReaction<TConfig, in TState, in TCommand>
		where TConfig : IConfig
		where TState : IState
		where TCommand : ICommand<TConfig, TState> {
		Task Before(TConfig config, TState state, TCommand command, CancellationToken cancellationToken);
		Task After(TConfig config, TState state, TCommand command, CancellationToken cancellationToken);
	}
}
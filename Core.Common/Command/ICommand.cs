using Core.Common.Config;
using Core.Common.State;

namespace Core.Common.Command {
	public interface ICommand<in TConfig, in TState>
		where TConfig : IConfig where TState : IState {
		CommandResult Apply(TConfig config, TState state);
	}
}
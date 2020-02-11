using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Client {
	public interface IClient<TConfig, TState> where TConfig : IConfig where TState : IState {
		TState State { get; }
		InitializationResult Initialize();
		CommandApplyResult Apply(ICommand<TConfig, TState> command);
	}
}
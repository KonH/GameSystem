using Core.Common.Config;
using Core.Common.State;
using Core.Service.Model;

namespace Core.Service.Queue {
	public interface IUpdateWatcher<TConfig, TState> where TConfig : IConfig where TState : IState {
		void TryAddCommands(UserId userId, TConfig config, TState state, CommandSet<TConfig, TState> commands);
	}
}
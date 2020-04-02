using Core.Common.Config;
using Core.Common.State;
using Core.Service.Model;

namespace Core.Service {
	public interface IUpdateWatcher<TConfig, TState> where TConfig : IConfig where TState : IState {
		void OnCommandRequest(UserId userId, CommandScheduler<TConfig, TState> scheduler);
	}
}
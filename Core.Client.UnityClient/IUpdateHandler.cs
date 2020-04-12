using Core.Common.Config;
using Core.Common.State;

namespace Core.Client.UnityClient {
	public interface IUpdateHandler<TConfig, TState> where TConfig : IConfig where TState : IState {
		void Update(TConfig config, TState state);
	}
}
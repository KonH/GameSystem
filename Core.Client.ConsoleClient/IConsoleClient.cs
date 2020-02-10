using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Client.ConsoleClient {
	public interface IConsoleClient<TConfig, TState> where TConfig : IConfig where TState : IState {
		TState State { get; }

		void Initialize();
		void Apply(ICommand<TConfig, TState> command);
	}
}
using System.Threading.Tasks;
using Core.Client.Shared;
using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Client.Abstractions {
	public interface IClient<TConfig, TState> where TConfig : IConfig where TState : IState {
		TState State { get; }
		Task<InitializationResult> Initialize();
		Task<CommandApplyResult> Apply(ICommand<TConfig, TState> command);
	}
}
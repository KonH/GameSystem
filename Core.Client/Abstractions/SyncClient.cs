using System.Threading.Tasks;
using Core.Client.Shared;
using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Client.Abstractions {
	public abstract class SyncClient<TConfig, TState> : IClient<TConfig, TState>
		where TConfig : IConfig where TState : IState {
		public TState State { get; protected set; }

		Task<InitializationResult> IClient<TConfig, TState>.Initialize() =>
			Task.FromResult(Initialize());

		protected abstract InitializationResult Initialize();

		Task<CommandApplyResult> IClient<TConfig, TState>.Apply(ICommand<TConfig, TState> command) =>
			Task.FromResult(Apply(command));

		protected abstract CommandApplyResult Apply(ICommand<TConfig, TState> command);
	}
}
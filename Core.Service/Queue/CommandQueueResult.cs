using Core.Common.Command;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Service.Queue {
	public sealed class CommandQueueResult<TConfig, TState> where TConfig : IConfig where TState : IState {
		public ICommand<TConfig, TState>[] Commands { get; set; }
		public BatchCommandResult[]        Errors   { get; set; }

		public CommandQueueResult(ICommand<TConfig, TState>[] commands, BatchCommandResult[] errors) {
			Commands = commands;
			Errors   = errors;
		}
	}
}
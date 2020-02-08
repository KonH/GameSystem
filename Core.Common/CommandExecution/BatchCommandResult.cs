using System.Collections.Generic;
using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Common.CommandExecution {
	public abstract class BatchCommandResult<TConfig, TState> where TConfig : IConfig where TState : IState {
		public sealed class OkResult : BatchCommandResult<TConfig, TState> {
			public List<ICommand<TConfig, TState>> NextCommands { get; set; } = new List<ICommand<TConfig, TState>>();

			public OkResult() { }

			public OkResult(List<ICommand<TConfig, TState>> nextCommands) {
				NextCommands = nextCommands;
			}
		}

		public class BadCommandResult : BatchCommandResult<TConfig, TState> {
			public string Description { get; set; }

			public BadCommandResult() { }

			public BadCommandResult(string description) {
				Description = description;
			}
		}

		public static BatchCommandResult<TConfig, TState> Ok(List<ICommand<TConfig, TState>> nextCommands) =>
			new OkResult(nextCommands);

		public static BatchCommandResult<TConfig, TState> BadCommand(string description) =>
			new BadCommandResult(description);
	}
}
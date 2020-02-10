using System.Collections.Generic;
using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Common.CommandExecution {
	public abstract class BatchCommandResult<TConfig, TState> where TConfig : IConfig where TState : IState {
		public sealed class Ok : BatchCommandResult<TConfig, TState> {
			public List<ICommand<TConfig, TState>> NextCommands { get; set; } = new List<ICommand<TConfig, TState>>();

			public Ok() { }

			public Ok(List<ICommand<TConfig, TState>> nextCommands) {
				NextCommands = nextCommands;
			}
		}

		public class BadCommand : BatchCommandResult<TConfig, TState> {
			public string Description { get; set; }

			public BadCommand() { }

			public BadCommand(string description) {
				Description = description;
			}
		}

		BatchCommandResult() { }
	}
}
using Core.Common.Command;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Service.UseCase.WaitCommand {
	public abstract class WaitCommandResponse {
		public sealed class Updated<TConfig, TState> : WaitCommandResponse
			where TConfig : IConfig where TState : IState {
			public StateVersion                NewVersion   { get; set; }
			public ICommand<TConfig, TState>[] NextCommands { get; set; }
			public BatchCommandResult[]        Errors       { get; set; }

			public Updated() {}

			public Updated(StateVersion newVersion, ICommand<TConfig, TState>[] nextCommands, BatchCommandResult[] errors) {
				NewVersion   = newVersion;
				NextCommands = nextCommands;
				Errors       = errors;
			}
		}

		public sealed class Outdated : WaitCommandResponse {}

		public sealed class NotFound : WaitCommandResponse {}

		public sealed class BadRequest : WaitCommandResponse {
			public string Description { get; set; }

			public BadRequest() {}

			public BadRequest(string description) {
				Description = description;
			}
		}

		WaitCommandResponse() {}
	}
}
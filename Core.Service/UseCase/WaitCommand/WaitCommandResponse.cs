using System.Collections.Generic;
using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Service.UseCase.WaitCommand {
	public abstract class WaitCommandResponse {
		public sealed class Updated<TConfig, TState> : WaitCommandResponse
			where TConfig : IConfig where TState : IState {
			public StateVersion                    NewVersion   { get; set; }
			public List<ICommand<TConfig, TState>> NextCommands { get; set; }

			public Updated() {}

			public Updated(StateVersion newVersion, List<ICommand<TConfig, TState>> nextCommands) {
				NewVersion   = newVersion;
				NextCommands = nextCommands;
			}
		}

		public sealed class Rejected : WaitCommandResponse {
			public string Description { get; set; }

			public Rejected() {}

			public Rejected(string description) {
				Description = description;
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
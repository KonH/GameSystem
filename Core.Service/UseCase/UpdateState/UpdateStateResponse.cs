using System.Collections.Generic;
using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Service.UseCase.UpdateState {
	public abstract class UpdateStateResponse<TConfig, TState> where TConfig : IConfig where TState : IState {
		public sealed class Updated : UpdateStateResponse<TConfig, TState> {
			public StateVersion                    NewVersion   { get; set; }
			public List<ICommand<TConfig, TState>> NextCommands { get; set; }

			public Updated() { }

			public Updated(StateVersion newVersion, List<ICommand<TConfig, TState>> nextCommands) {
				NewVersion   = newVersion;
				NextCommands = nextCommands;
			}
		}

		public sealed class Rejected : UpdateStateResponse<TConfig, TState> {
			public string Description { get; set; }

			public Rejected() { }

			public Rejected(string description) {
				Description = description;
			}
		}

		public sealed class NotFound : UpdateStateResponse<TConfig, TState> { }

		public sealed class Outdated : UpdateStateResponse<TConfig, TState> { }

		public sealed class BadRequest : UpdateStateResponse<TConfig, TState> {
			public string Description { get; set; }

			public BadRequest() { }

			public BadRequest(string description) {
				Description = description;
			}
		}

		UpdateStateResponse() { }
	}
}
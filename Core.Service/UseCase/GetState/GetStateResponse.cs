using Core.Common.State;

namespace Core.Service.UseCase.GetState {
	public abstract class GetStateResponse {
		public sealed class Found<TState> : GetStateResponse where TState : IState {
			public TState State { get; set; }

			public Found() { }

			public Found(TState state) {
				State = state;
			}
		}

		public sealed class NotFound : GetStateResponse { }

		public sealed class BadRequest : GetStateResponse {
			public string Description { get; set; }

			public BadRequest() { }

			public BadRequest(string description) {
				Description = description;
			}
		}

		GetStateResponse() { }
	}
}
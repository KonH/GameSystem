using Core.Common.State;

namespace Core.Service.UseCase.GetState {
	public abstract class GetStateResponse<TState> where TState : IState {
		public sealed class Found : GetStateResponse<TState> {
			public TState State { get; set; }

			public Found() { }

			public Found(TState state) {
				State = state;
			}
		}

		public sealed class NotFound : GetStateResponse<TState> { }

		public sealed class BadRequest : GetStateResponse<TState> {
			public string Description { get; set; }

			public BadRequest() { }

			public BadRequest(string description) {
				Description = description;
			}
		}

		GetStateResponse() { }
	}
}
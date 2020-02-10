using Core.Service.Model;

namespace Core.Service.UseCase.GetState {
	public sealed class GetStateRequest {
		public UserId UserId { get; set; }

		public GetStateRequest() { }

		public GetStateRequest(UserId userId) {
			UserId = userId;
		}
	}
}
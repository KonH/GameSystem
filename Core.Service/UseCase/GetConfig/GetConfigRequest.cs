using Core.Service.Model;

namespace Core.Service.UseCase.GetConfig {
	public sealed class GetConfigRequest {
		public UserId UserId { get; set; }

		public GetConfigRequest() { }

		public GetConfigRequest(UserId userId) {
			UserId = userId;
		}
	}
}
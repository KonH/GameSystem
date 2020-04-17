namespace Core.Service.UseCase.SendCommand {
	public abstract class SendCommandResponse {
		public sealed class Applied : SendCommandResponse {}

		public sealed class Rejected : SendCommandResponse {
			public string Description { get; set; }

			public Rejected() {}

			public Rejected(string description) {
				Description = description;
			}
		}

		public sealed class BadRequest : SendCommandResponse {
			public string Description { get; set; }

			public BadRequest() {}

			public BadRequest(string description) {
				Description = description;
			}
		}

		SendCommandResponse() {}
	}
}
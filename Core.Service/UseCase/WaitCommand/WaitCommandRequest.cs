using Core.Common.Config;
using Core.Common.State;
using Core.Service.Model;

namespace Core.Service.UseCase.WaitCommand {
	public sealed class WaitCommandRequest {
		public UserId        UserId        { get; set; }
		public StateVersion  StateVersion  { get; set; }
		public ConfigVersion ConfigVersion { get; set; }

		public WaitCommandRequest() {}

		public WaitCommandRequest(UserId userId, StateVersion stateVersion, ConfigVersion configVersion) {
			UserId        = userId;
			StateVersion  = stateVersion;
			ConfigVersion = configVersion;
		}
	}
}
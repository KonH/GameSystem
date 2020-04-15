using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;
using Core.Service.Model;

namespace Core.Service.UseCase.SendCommand {
	public sealed class SendCommandRequest<TConfig, TState> where TConfig : IConfig where TState : IState {
		public UserId                    UserId        { get; set; }
		public StateVersion              StateVersion  { get; set; }
		public ConfigVersion             ConfigVersion { get; set; }
		public ICommand<TConfig, TState> Command       { get; set; }

		public SendCommandRequest() {}

		public SendCommandRequest(
			UserId userId, StateVersion stateVersion, ConfigVersion configVersion,
			ICommand<TConfig, TState> command) {
			UserId        = userId;
			StateVersion  = stateVersion;
			ConfigVersion = configVersion;
			Command       = command;
		}
	}
}
using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;
using Core.Service.Model;

namespace Core.Service.UseCase.UpdateState {
	public sealed class UpdateStateRequest<TConfig, TState> where TConfig : IConfig where TState : IState {
		public UserId                    UserId        { get; set; }
		public StateVersion              StateVersion  { get; set; }
		public ConfigVersion             ConfigVersion { get; set; }
		public ICommand<TConfig, TState> Command       { get; set; }

		public UpdateStateRequest() { }

		public UpdateStateRequest(
			UserId userId, StateVersion stateVersion, ConfigVersion configVersion,
			ICommand<TConfig, TState> command) {
			UserId        = userId;
			StateVersion  = stateVersion;
			ConfigVersion = configVersion;
			Command       = command;
		}
	}
}
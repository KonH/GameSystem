using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Common.CommandExecution {
	public sealed class CommandExecutor<TConfig, TState>
		where TState : IState where TConfig : IConfig {
		public CommandResult Apply<TCommand>(TConfig config, TState state, TCommand command)
			where TCommand : ICommand<TConfig, TState> {
			if ( config == null ) {
				return CommandResult.BadCommand("Config is invalid");
			}
			if ( state == null ) {
				return CommandResult.BadCommand("State is invalid");
			}
			if ( command == null ) {
				return CommandResult.BadCommand("Command is invalid");
			}
			var commandResult = command.Apply(config, state);
			if ( commandResult is CommandResult.OkResult ) {
				state.Version = new StateVersion(state.Version.Value + 1);
			}
			return commandResult;
		}
	}
}
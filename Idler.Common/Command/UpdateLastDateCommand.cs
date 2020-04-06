using System;
using Core.Common.Command;
using Idler.Common.Config;
using Idler.Common.State;

namespace Idler.Common.Command {
	public sealed class UpdateLastDateCommand : ICommand<GameConfig, GameState> {
		public DateTimeOffset LastDate { get; set; }

		public UpdateLastDateCommand() { }

		public UpdateLastDateCommand(DateTimeOffset lastDate) {
			LastDate = lastDate;
		}

		public CommandResult Apply(GameConfig config, GameState state) {
			if ( LastDate < state.Time.LastDate ) {
				return CommandResult.BadCommand($"Invalid last date: {LastDate}, but current is {state.Time.LastDate}");
			}
			state.Time.LastDate = LastDate;
			return CommandResult.Ok();
		}

		public override string ToString() {
			return $"{nameof(UpdateLastDateCommand)}: {LastDate}";
		}
	}
}
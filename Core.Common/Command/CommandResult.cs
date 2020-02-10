namespace Core.Common.Command {
	public abstract class CommandResult {
		public sealed class OkResult : CommandResult { }

		public sealed class BadCommandResult : CommandResult {
			public string Description { get; set; }

			public BadCommandResult() { }

			public BadCommandResult(string description) {
				Description = description;
			}
		}

		CommandResult() { }

		public static CommandResult Ok() => new OkResult();

		public static CommandResult BadCommand(string description) => new BadCommandResult(description);
	}
}
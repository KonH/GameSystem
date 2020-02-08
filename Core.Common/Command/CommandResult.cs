namespace Core.Common.Command {
	public abstract class CommandResult {
		public class OkResult : CommandResult { }

		public class BadCommandResult : CommandResult {
			public string Description { get; set; }

			public BadCommandResult() { }

			public BadCommandResult(string description) {
				Description = description;
			}
		}

		public static CommandResult Ok() => new OkResult();

		public static CommandResult BadCommand(string description) => new BadCommandResult(description);
	}
}
namespace Core.Client {
	public abstract class CommandApplyResult {
		public sealed class Ok : CommandApplyResult { }

		public sealed class Error : CommandApplyResult {
			public readonly string Description;

			public Error(string description) {
				Description = description;
			}
		}

		CommandApplyResult() { }
	}
}
namespace Core.Client {
	public abstract class InitializationResult {
		public sealed class Ok : InitializationResult { }

		public sealed class Error : InitializationResult {
			public readonly string Description;

			public Error(string description) {
				Description = description;
			}
		}

		InitializationResult() { }
	}
}
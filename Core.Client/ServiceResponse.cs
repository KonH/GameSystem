namespace Core.Client {
	public abstract class ServiceResponse {
		public sealed class Ok<T> : ServiceResponse {
			public readonly T Result;

			public Ok(T result) {
				Result = result;
			}
		}

		public sealed class Error : ServiceResponse {
			public readonly string Description;

			public Error(string description) {
				Description = description;
			}
		}
	}
}
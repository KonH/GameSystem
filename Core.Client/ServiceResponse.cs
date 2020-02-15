namespace Core.Client {
	public abstract class ServiceResponse<T> {
		public sealed class Ok : ServiceResponse<T> {
			public readonly T Result;

			public Ok(T result) {
				Result = result;
			}
		}

		public sealed class Error : ServiceResponse<T> {
			public readonly string Description;

			public Error(string description) {
				Description = description;
			}
		}
	}
}
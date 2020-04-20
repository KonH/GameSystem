using Idler.Common.State;

namespace Idler.Common.Repository {
	public sealed class SharedStateRepository {
		SharedState _state = new SharedState();

		public SharedState Get() {
			return _state;
		}

		public void Update(SharedState state) {
			_state = state;
		}
	}
}
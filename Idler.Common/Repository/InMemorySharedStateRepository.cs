using System.Threading.Tasks;
using Idler.Common.State;

namespace Idler.Common.Repository {
	public sealed class InMemorySharedStateRepository : ISharedStateRepository {
		SharedState _state = new SharedState();

		public Task<SharedState> Get() {
			return Task.FromResult(_state);
		}

		public Task Update(SharedState state) {
			_state = state;
			return Task.CompletedTask;
		}
	}
}
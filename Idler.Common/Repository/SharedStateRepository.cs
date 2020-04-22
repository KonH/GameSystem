using Idler.Common.State;

namespace Idler.Common.Repository {
	public interface ISharedStateRepository {
		SharedState Get();
		void Update(SharedState state);
	}
}
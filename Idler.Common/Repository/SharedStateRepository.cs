using System.Threading.Tasks;
using Idler.Common.State;

namespace Idler.Common.Repository {
	public interface ISharedStateRepository {
		Task<SharedState> Get();
		Task Update(SharedState state);
	}
}
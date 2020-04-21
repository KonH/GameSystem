using System.Threading.Tasks;

namespace Idler.UnityClient.Repository {
	public interface ISharedStateProvider {
		Task<int> GetResourceCount();
	}
}
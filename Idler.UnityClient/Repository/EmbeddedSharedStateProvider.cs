using System.Threading.Tasks;
using Idler.Common.Repository;

namespace Idler.UnityClient.Repository {
	public sealed class EmbeddedSharedStateProvider : ISharedStateProvider {
		readonly ISharedStateRepository _repository;

		public EmbeddedSharedStateProvider(ISharedStateRepository repository) {
			_repository = repository;
		}

		public Task<int> GetResourceCount() {
			return Task.FromResult(_repository.Get().Resources);
		}
	}
}
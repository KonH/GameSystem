using System.Threading.Tasks;
using Idler.Common.Repository;

namespace Idler.UnityClient.Repository {
	public sealed class EmbeddedSharedStateProvider : ISharedStateProvider {
		readonly SharedStateRepository _repository;

		public EmbeddedSharedStateProvider(SharedStateRepository repository) {
			_repository = repository;
		}

		public Task<int> GetResourceCount() {
			return Task.FromResult(_repository.Get().Resources);
		}
	}
}
using System.Threading.Tasks;

namespace Core.Service.Repository {
	public class InMemoryRepositoryDecorator<TModel, TStorage> : IRepository<TModel> {
		public delegate TStorage ConvertToStorage(TModel model);

		public delegate TModel ConvertToModel(TStorage storage);

		public sealed class Settings {
			public readonly ConvertToStorage ConvertToStorage;
			public readonly ConvertToModel   ConvertToModel;

			public Settings(ConvertToStorage convertToStorage, ConvertToModel convertToModel) {
				ConvertToStorage = convertToStorage;
				ConvertToModel   = convertToModel;
			}
		}

		readonly Settings _settings;

		readonly InMemoryRepository<TStorage> _repository = new InMemoryRepository<TStorage>();

		public InMemoryRepositoryDecorator(Settings settings) {
			_settings = settings;
		}

		public Task Add(string id, TModel model) {
			var storage = _settings.ConvertToStorage(model);
			_repository.Add(id, storage);
			return Task.CompletedTask;
		}

		public async Task<TModel> Get(string id) {
			var storage = await _repository.Get(id);
			if ( storage == null ) {
				return default;
			}
			var model = _settings.ConvertToModel(storage);
			return model;
		}

		public Task Update(string id, TModel model) {
			var storage = _settings.ConvertToStorage(model);
			_repository.Update(id, storage);
			return Task.CompletedTask;
		}
	}
}
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

		public void Add(string id, TModel model) {
			var storage = _settings.ConvertToStorage(model);
			_repository.Add(id, storage);
		}

		public TModel Get(string id) {
			var storage = _repository.Get(id);
			if ( storage == null ) {
				return default;
			}
			var model = _settings.ConvertToModel(storage);
			return model;
		}

		public void Update(string id, TModel model) {
			var storage = _settings.ConvertToStorage(model);
			_repository.Update(id, storage);
		}
	}
}
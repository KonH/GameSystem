using System.Collections.Generic;
using Core.Common.Extension;

namespace Core.Service.Repository {
	public sealed class InMemoryRepository<TModel> : IRepository<TModel> {
		readonly Dictionary<string, TModel> _models = new Dictionary<string, TModel>();

		public void Add(string id, TModel model) {
			_models.Add(id, model);
		}

		public TModel Get(string id) {
			return _models.GetOrDefault(id);
		}

		public void Update(string id, TModel model) {
			_models[id] = model;
		}
	}
}
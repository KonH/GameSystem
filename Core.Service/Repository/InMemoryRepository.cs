using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Common.Extension;

namespace Core.Service.Repository {
	public sealed class InMemoryRepository<TModel> : IRepository<TModel> {
		readonly Dictionary<string, TModel> _models = new Dictionary<string, TModel>();

		public Task Add(string id, TModel model) {
			_models.Add(id, model);
			return Task.CompletedTask;
		}

		public Task<TModel> Get(string id) {
			return Task.FromResult(_models.GetOrDefault(id));
		}

		public Task Update(string id, TModel model) {
			_models[id] = model;
			return Task.CompletedTask;
		}
	}
}
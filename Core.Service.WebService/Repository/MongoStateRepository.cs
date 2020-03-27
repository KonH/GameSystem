using System.Threading.Tasks;
using Core.Common.State;
using Core.Service.Repository.State;
using Core.Service.WebService.Configuration;
using Core.Service.WebService.Models;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Core.Service.WebService.Repository {
	public sealed class MongoStateRepository<TState> : IStateRepository<TState> where TState : IState {
		readonly IMongoCollection<MongoState> _items;

		public MongoStateRepository(Settings settings) {
			var client   = new MongoClient(settings.MongoConnectionString);
			var database = client.GetDatabase(settings.MongoDatabaseName);

			_items = database.GetCollection<MongoState>("State");
		}

		public Task Add(string id, TState model) {
			var state = CreateState(id, model);
			return Add(state);
		}

		public async Task<TState> Get(string id) {
			var state = (await _items.FindAsync(s => s.Id == id)).FirstOrDefault();
			if ( state == null ) {
				return default;
			}
			return JsonConvert.DeserializeObject<TState>(state.Body);
		}

		public async Task Update(string id, TState model) {
			var state  = CreateState(id, model);
			var result = await _items.ReplaceOneAsync(s => s.Id == id, state);
			if ( result.ModifiedCount == 0 ) {
				await Add(state);
			}
		}

		Task Add(MongoState state) {
			return _items.InsertOneAsync(state);
		}

		MongoState CreateState(string id, TState model) {
			return new MongoState {
				Id   = id,
				Body = JsonConvert.SerializeObject(model)
			};
		}
	}
}
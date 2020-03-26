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

		public void Add(string id, TState model) {
			var state = CreateState(id, model);
			Add(state);
		}

		public TState Get(string id) {
			var state = _items.Find(s => s.Id == id).Limit(0).FirstOrDefault();
			if ( state == null ) {
				return default;
			}
			return JsonConvert.DeserializeObject<TState>(state.Body);
		}

		public void Update(string id, TState model) {
			var state  = CreateState(id, model);
			var result = _items.ReplaceOne(s => s.Id == id, state);
			if ( result.ModifiedCount == 0 ) {
				Add(state);
			}
		}

		void Add(MongoState state) {
			_items.InsertOne(state);
		}

		MongoState CreateState(string id, TState model) {
			return new MongoState {
				Id   = id,
				Body = JsonConvert.SerializeObject(model)
			};
		}
	}
}
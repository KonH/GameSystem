using Core.Common.State;
using Core.Service.Repository.State;
using Core.Service.WebService.Configuration;
using Core.Service.WebService.Models;
using CouchDB.Driver;
using Newtonsoft.Json;

namespace Core.Service.WebService.Repository {
	public sealed class CouchStateRepository<TState> : IStateRepository<TState> where TState : IState {
		readonly CouchDatabase<CouchState> _database;

		public CouchStateRepository(Settings settings) {
			var client = new CouchClient(settings.CouchConnectionString);
			_database = client.GetDatabase<CouchState>(settings.CouchDatabaseName);
		}

		public void Add(string id, TState model) {
			var state = CreateState(id, model);
			Add(state);
		}

		public TState Get(string id) {
			var state = GetRawState(id);
			if ( state == null ) {
				return default;
			}
			return JsonConvert.DeserializeObject<TState>(state.Body);
		}

		public void Update(string id, TState model) {
			var state = GetRawState(id);
			if ( state == null ) {
				state = CreateState(id, model);
			} else {
				state.Body = CreateBody(model);
			}
			_database.CreateOrUpdateAsync(state).GetAwaiter().GetResult();
		}

		CouchState GetRawState(string id) {
			return _database.FindAsync(id).GetAwaiter().GetResult();
		}

		void Add(CouchState state) {
			_database.CreateAsync(state).GetAwaiter().GetResult();
		}

		CouchState CreateState(string id, TState model) {
			return new CouchState {
				Id   = id,
				Body = CreateBody(model)
			};
		}

		string CreateBody(TState model) {
			return JsonConvert.SerializeObject(model);
		}
	}
}
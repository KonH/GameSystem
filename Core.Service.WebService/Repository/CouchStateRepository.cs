using System.Threading.Tasks;
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

		public Task Add(string id, TState model) {
			var state = CreateState(id, model);
			return Add(state);
		}

		public async Task<TState> Get(string id) {
			var state = await GetRawState(id);
			if ( state == null ) {
				return default;
			}
			return JsonConvert.DeserializeObject<TState>(state.Body);
		}

		public async Task Update(string id, TState model) {
			var state = await GetRawState(id);
			if ( state == null ) {
				state = CreateState(id, model);
			} else {
				state.Body = CreateBody(model);
			}
			await _database.CreateOrUpdateAsync(state);
		}

		Task<CouchState> GetRawState(string id) {
			return _database.FindAsync(id);
		}

		Task Add(CouchState state) {
			return _database.CreateAsync(state);
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
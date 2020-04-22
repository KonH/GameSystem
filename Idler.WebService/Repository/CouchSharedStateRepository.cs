using System.Threading.Tasks;
using Core.Service.WebService.Configuration;
using Core.Service.WebService.Models;
using CouchDB.Driver;
using Idler.Common.Repository;
using Idler.Common.State;
using Newtonsoft.Json;

namespace Idler.WebService.Repository {
	public sealed class CouchSharedStateRepository : ISharedStateRepository {
		readonly string _id = "shared_state";

		readonly CouchDatabase<CouchState> _database;

		public CouchSharedStateRepository(Settings settings) {
			var client = new CouchClient(settings.CouchConnectionString);
			_database = client.GetDatabase<CouchState>(settings.CouchDatabaseName);
		}

		public SharedState Get() {
			var state = /*await*/ GetRawState(_id).GetAwaiter().GetResult();
			if ( state == null ) {
				return new SharedState();
			}
			return JsonConvert.DeserializeObject<SharedState>(state.Body);
		}

		public void Update(SharedState model) {
			var state = /*await*/ GetRawState(_id).GetAwaiter().GetResult();
			if ( state == null ) {
				state = CreateState(model);
			} else {
				state.Body = CreateBody(model);
			}
			/*await*/ _database.CreateOrUpdateAsync(state).GetAwaiter().GetResult();
		}

		Task<CouchState> GetRawState(string id) {
			return _database.FindAsync(id);
		}

		CouchState CreateState(SharedState model) {
			return new CouchState {
				Id   = _id,
				Body = CreateBody(model)
			};
		}

		string CreateBody(SharedState model) {
			return JsonConvert.SerializeObject(model);
		}
	}
}
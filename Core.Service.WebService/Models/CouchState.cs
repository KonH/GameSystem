using CouchDB.Driver.Types;

namespace Core.Service.WebService.Models {
	public sealed class CouchState : CouchDocument {
		public string Body { get; set; }
	}
}
using System;
using Core.Service.Model;

namespace Core.Client.Shared {
	public sealed class UserIdGenerator {
		public UserId GetNewUserId() {
			var guid = Guid.NewGuid();
			return new UserId(guid.ToString());
		}
	}
}
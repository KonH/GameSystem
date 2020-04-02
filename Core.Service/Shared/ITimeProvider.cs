using System;

namespace Core.Service.Shared {
	public interface ITimeProvider {
		DateTimeOffset UtcNow { get; }
	}
}
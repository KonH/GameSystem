using System;

namespace Core.Service.Shared {
	public sealed class RealTimeProvider : ITimeProvider {
		public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
	}
}
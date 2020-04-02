using System;

namespace Core.Service.Shared {
	public sealed class FixedTimeProvider : ITimeProvider {
		public DateTimeOffset UtcNow { get; private set; } = DateTimeOffset.UtcNow;

		public void Advance(TimeSpan span) {
			UtcNow = UtcNow.Add(span);
		}
	}
}
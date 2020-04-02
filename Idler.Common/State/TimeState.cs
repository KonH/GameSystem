using System;

namespace Idler.Common.State {
	public sealed class TimeState {
		public DateTimeOffset LastDate { get; set; }

		public TimeState() {}

		public TimeState(DateTimeOffset lastDate) {
			LastDate = lastDate;
		}
	}
}
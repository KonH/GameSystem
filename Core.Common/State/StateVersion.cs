using System;

namespace Core.Common.State {
	public sealed class StateVersion : IComparable<StateVersion> {
		public int Value { get; set; }

		public StateVersion() { }

		public StateVersion(int value) {
			Value = value;
		}

		public override string ToString() {
			return Value.ToString();
		}

		public override bool Equals(object obj) {
			return ReferenceEquals(this, obj) || obj is StateVersion other && Equals(other);
		}

		public override int GetHashCode() {
			return Value;
		}

		public int CompareTo(StateVersion other) {
			if ( ReferenceEquals(this, other) ) {
				return 0;
			}
			if ( ReferenceEquals(null, other) ) {
				return 1;
			}
			return Value.CompareTo(other.Value);
		}

		bool Equals(StateVersion other) {
			return Value == other.Value;
		}
	}
}
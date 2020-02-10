namespace Core.Service.Model {
	public sealed class UserId {
		public string Value { get; set; }

		public UserId() { }

		public UserId(string value) {
			Value = value;
		}

		public override string ToString() {
			return Value;
		}

		public override bool Equals(object obj) {
			return ReferenceEquals(this, obj) || obj is UserId other && Equals(other);
		}

		public override int GetHashCode() {
			return (Value != null ? Value.GetHashCode() : 0);
		}

		bool Equals(UserId other) {
			return Value == other.Value;
		}
	}
}
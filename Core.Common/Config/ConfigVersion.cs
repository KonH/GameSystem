namespace Core.Common.Config {
	public sealed class ConfigVersion {
		public string Value { get; set; }

		public ConfigVersion() { }

		public ConfigVersion(string value) {
			Value = value;
		}

		public override bool Equals(object obj) {
			return ReferenceEquals(this, obj) || obj is ConfigVersion other && Equals(other);
		}

		public override int GetHashCode() {
			return (Value != null ? Value.GetHashCode() : 0);
		}

		bool Equals(ConfigVersion other) {
			return Value == other.Value;
		}
	}
}
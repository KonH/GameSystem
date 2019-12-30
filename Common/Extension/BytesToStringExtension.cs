using System.Text;

namespace Common.Extension {
	public static class BytesToStringExtension {
		static StringBuilder _sb = new StringBuilder();

		public static string ToSizeString(this int totalBytes, bool includeAllDimensions = false) =>
			ToSizeString((long)totalBytes, includeAllDimensions);

		public static string ToSizeString(this long totalBytes, bool includeAllDimensions = false) {
			var sb = _sb.Clear();

			var bytes = totalBytes;
			var anyGb = TryWriteGb(sb, ref bytes);
			if ( anyGb && !includeAllDimensions ) {
				return sb.ToString();
			}
			var anyMb = TryWriteMb(sb, anyGb, ref bytes);
			if ( anyMb && !includeAllDimensions ) {
				return sb.ToString();
			}
			var anyKb = TryWriteKb(sb, anyMb, ref bytes);
			if ( anyKb && !includeAllDimensions ) {
				return sb.ToString();
			}
			if ( (bytes > 0) || (totalBytes <= 0) ) {
				var anyValues = anyGb || anyMb || anyKb;
				WriteBytes(sb, anyValues, bytes);
			}

			return sb.ToString();
		}

		static bool TryWriteGb(StringBuilder sb, ref long bytes) {
			return TryWrite(sb, "GB", ByteConvertExtension.BytesInGb, false, ref bytes);
		}

		static bool TryWriteMb(StringBuilder sb, bool hasHigherDimension, ref long bytes) {
			return TryWrite(sb, "MB", ByteConvertExtension.BytesInMb, hasHigherDimension, ref bytes);
		}

		static bool TryWriteKb(StringBuilder sb, bool hasHigherDimension, ref long bytes) {
			return TryWrite(sb, "KB", ByteConvertExtension.BytesInKb, hasHigherDimension, ref bytes);
		}

		static bool TryWrite(StringBuilder sb, string name, long div, bool hasHigherDimension, ref long bytes) {
			var divValue = bytes / div;
			if ( divValue == 0 ) {
				return false;
			}
			TryAddSpace(sb, hasHigherDimension);
			sb.Append(divValue).Append(" ").Append(name);
			bytes %= div;
			return true;
		}

		static void WriteBytes(StringBuilder sb, bool hasHigherDimension, long bytes) {
			TryAddSpace(sb, hasHigherDimension);
			sb.Append(bytes).Append(" B");
		}

		static void TryAddSpace(StringBuilder sb, bool condition) {
			if ( condition ) {
				sb.Append(" ");
			}
		}
	}
}
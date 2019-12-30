namespace Common.Extension {
	public static class ByteConvertExtension {
		public const int BytesInKb = 1024;
		public const int BytesInMb = BytesInKb * 1024;
		public const int BytesInGb = BytesInMb * 1024;

		public static long Kb(this int count) => Kb((long)count);
		public static long Mb(this int count) => Mb((long)count);
		public static long Gb(this int count) => Gb((long)count);

		public static long Kb(this long count) {
			return count * BytesInKb;
		}

		public static long Mb(this long count) {
			return count * BytesInMb;
		}

		public static long Gb(this long count) {
			return count * BytesInGb;
		}

		public static long GetKbCount(this int count) => GetKbCount((long)count);
		public static long GetMbCount(this int count) => GetMbCount((long)count);
		public static long GetGbCount(this int count) => GetGbCount((long)count);

		public static long GetKbCount(this long bytes) {
			return bytes / BytesInKb;
		}

		public static long GetMbCount(this long bytes) {
			return bytes / BytesInMb;
		}

		public static long GetGbCount(this long bytes) {
			return bytes / BytesInGb;
		}
	}
}
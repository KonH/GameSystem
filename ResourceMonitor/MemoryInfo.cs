namespace ResourceMonitor {
	struct MemoryInfo {
		public readonly long Free;
		public readonly long Total;

		public long Used => (Total - Free);

		public MemoryInfo(long free, long total) {
			Free  = free;
			Total = total;
		}
	}
}
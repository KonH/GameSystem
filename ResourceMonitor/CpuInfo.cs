namespace ResourceMonitor {
	struct CpuInfo {
		public readonly long User;
		public readonly long Nice;
		public readonly long System;
		public readonly long Total;

		public long Work => (User + Nice + System);

		public CpuInfo(long user, long nice, long system, long total) {
			User = user;
			Nice = nice;
			System = system;
			Total = total;
		}
	}
}
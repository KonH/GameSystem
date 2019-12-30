namespace ResourceMonitor {
	struct SystemInfo {
		public long   UsedMemory;
		public long   UnusedMemory;
		public bool   IsMemoryParsed;
		public double UserCpuUsage;
		public double SystemCpuUsage;
		public bool   IsCpuParsed;

		public long TotalMemoryUsage => (UsedMemory + UnusedMemory);
		public double RelativeMemoryUsage => (float)UsedMemory / TotalMemoryUsage;
		public double TotalCpuUsage => (UserCpuUsage + SystemCpuUsage);

		public bool IsAllParsed => IsMemoryParsed && IsCpuParsed;
	}
}
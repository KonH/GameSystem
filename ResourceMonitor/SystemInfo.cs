namespace ResourceMonitor {
	struct SystemInfo {
		public readonly double CpuUsage;
		public readonly double MemoryUsage;

		public SystemInfo(CpuInfo prevCpu, CpuInfo curCpu, MemoryInfo memory) {
			var workForPeriod = curCpu.Work - prevCpu.Work;
			var totalForPeriod = curCpu.Total - prevCpu.Total;
			CpuUsage = (double)workForPeriod / totalForPeriod;
			MemoryUsage = (double)(memory.Total - memory.Free) / (memory.Total);
		}
	}
}
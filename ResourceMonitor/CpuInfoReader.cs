using System;
using System.IO;
using System.Linq;

namespace ResourceMonitor {
	static class CpuInfoReader {
		public static CpuInfo Read() {
			var procStatContent = File.ReadLines("/proc/stat");

			var cpuLine = procStatContent.First();
			var parts   = cpuLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			var nums    = parts.Skip(1).Select(long.Parse).ToArray();
			var user    = nums[0];
			var nice    = nums[1];
			var system  = nums[2];
			var total   = nums.Sum();
			return new CpuInfo(user, nice, system, total);
		}
	}
}
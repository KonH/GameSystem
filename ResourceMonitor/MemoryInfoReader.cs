using System;
using System.IO;
using System.Linq;
using Core.Common.Extension;

namespace ResourceMonitor {
	static class MemoryInfoReader {
		public static MemoryInfo Read() {
			var procMemInfoContent = File.ReadLines("/proc/meminfo");

			var firstLines = procMemInfoContent.Take(2).ToArray();
			var totalParts = firstLines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
			var freeParts  = firstLines[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
			var totalKbs   = long.Parse(totalParts[1]);
			var freeKbs    = long.Parse(freeParts[1]);
			return new MemoryInfo(freeKbs.Kb(), totalKbs.Kb());
		}
	}
}
using System;
using System.Threading.Tasks;

namespace ResourceMonitor {
	static class Program {
		const int Interval = 3000;

		static async Task Main() {
			var writer = new ConsoleWriter();
			var prevCpu = CpuInfoReader.Read();
			while ( true ) {
				if ( Console.KeyAvailable ) {
					return;
				}
				await Task.Delay(Interval);
				var curCpu = CpuInfoReader.Read();
				var memory = MemoryInfoReader.Read();
				var info = new SystemInfo(prevCpu, curCpu, memory);
				writer.Write(info);
				prevCpu = curCpu;
			}
		}
	}
}
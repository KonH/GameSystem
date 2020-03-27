using System;
using System.Threading.Tasks;
using ResourceMonitor.Writer;

namespace ResourceMonitor {
	static class Program {
		const int Interval = 3000;

		static async Task Main() {
			using var writer = new CompositeWriter(new ConsoleWriter(), new PiWriter());
			AppDomain.CurrentDomain.ProcessExit += (sender, args) => writer.Dispose();
			var prevCpu = CpuInfoReader.Read();
			while ( true ) {
				await Task.Delay(Interval);
				var curCpu = CpuInfoReader.Read();
				var memory = MemoryInfoReader.Read();
				var info   = new SystemInfo(prevCpu, curCpu, memory);
				writer.Write(info);
				prevCpu = curCpu;
			}
			// ReSharper disable once FunctionNeverReturns
		}
	}
}
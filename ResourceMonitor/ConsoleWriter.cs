using System;

namespace ResourceMonitor {
	sealed class ConsoleWriter : IWriter {
		const int ColumnCount = 10;

		public void Write(SystemInfo info) {
			WritePercent("CPU", info.CpuUsage);
			WritePercent("MEM", info.MemoryUsage);
			Console.WriteLine();
		}

		void WritePercent(string header, double value) {
			var usedCount = (int)Math.Round(value * ColumnCount);
			var freeCount = ColumnCount - usedCount;
			Console.WriteLine($"{header} [{new string('=', usedCount)}{new string(' ', freeCount)}] {value:P2}");
		}
	}
}
using System;

namespace ResourceMonitor.Writer {
	sealed class ConsoleWriter : IWriter {
		const int ColumnCount = 10;

		public void Write(SystemInfo info) {
			WritePercent("CPU", info.CpuUsage);
			WritePercent("MEM", info.MemoryUsage);
			Console.WriteLine();
		}

		void WritePercent(string header, double value) {
			var usedCount = (int)Math.Round(value * ColumnCount);
			var freeCount = Math.Max(ColumnCount - usedCount, 0);
			Console.WriteLine($"{header} [{new string('=', usedCount)}{new string(' ', freeCount)}] {value:P2}");
		}
	}
}
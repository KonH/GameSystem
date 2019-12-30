using System;
using System.Threading.Tasks;
using Common.Utils;

namespace ResourceMonitor {
	static class Program {
		const int Interval = 3000;

		static async Task Main() {
			var writer = new ConsoleWriter();
			while ( true ) {
				// Read top output once
				var output = ProgramRunner.StartAndReadLines("top", "-l 1");
				var info = TopParser.GetSystemInfo(output);
				writer.Write(info);
				if ( Console.KeyAvailable ) {
					return;
				}
				await Task.Delay(Interval);
			}
		}
	}
}
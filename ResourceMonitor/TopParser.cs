using System;
using Common.Extension;

namespace ResourceMonitor {
	static class TopParser {
		public static SystemInfo GetSystemInfo(string[] lines) {
			var result = new SystemInfo();
			foreach ( var line in lines ) {
				ConsumeOutput(line, ref result);
				if ( result.IsAllParsed ) {
					break;
				}
			}
			return result;
		}

		static void ConsumeOutput(string line, ref SystemInfo info) {
			if ( line.StartsWith("PhysMem:") ) {
				ParseMemoryUsage(line.Split(' '), ref info);
			} else if ( line.StartsWith("CPU usage:") ) {
				ParseCpuUsage(line.Split(' '), ref info);
			}
		}

		static void ParseMemoryUsage(string[] tokens, ref SystemInfo info) {
			// PhysMem: 8007M used (1563M wired), 184M unused.
			var usedValueIndex = Array.FindIndex(tokens, s => (s == "used")) - 1;
			if ( usedValueIndex > 0 ) {
				info.UsedMemory = ParseMemoryValue(tokens[usedValueIndex]);
			}
			var unusedValueIndex = Array.FindIndex(tokens, s => (s == "unused.")) - 1;
			if ( unusedValueIndex > 0 ) {
				info.UnusedMemory = ParseMemoryValue(tokens[unusedValueIndex]);
			}
			info.IsMemoryParsed = true;
		}

		static void ParseCpuUsage(string[] tokens, ref SystemInfo info) {
			// CPU usage: 32.0% user, 37.0% sys, 31.0% idle
			var userValueIndex = Array.FindIndex(tokens, s => (s == "user,")) - 1;
			if ( userValueIndex > 0 ) {
				info.UserCpuUsage = ParsePercentValue(tokens[userValueIndex]);
			}
			var systemValueIndex = Array.FindIndex(tokens, s => (s == "sys,")) - 1;
			if ( systemValueIndex > 0 ) {
				info.SystemCpuUsage = ParsePercentValue(tokens[systemValueIndex]);
			}
			info.IsCpuParsed = true;
		}

		static long ParseMemoryValue(string value) {
			var lastSymbol = value[^1];
			return lastSymbol switch {
				'M' => long.Parse(value.Substring(0, value.Length - 1)).Mb(),
				'G' => long.Parse(value.Substring(0, value.Length - 1)).Gb(),
				_ => long.Parse(value)
			};
		}

		static double ParsePercentValue(string value) {
			return double.Parse(value.Substring(0, value.Length - 1)) / 100;
		}
	}
}
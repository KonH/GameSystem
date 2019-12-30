using System.Diagnostics;

namespace Common.Utils {
	public static class ProgramRunner {
		public static string StartAndReadOutput(string fileName, string arguments) {
			var startInfo = new ProcessStartInfo {
				FileName = fileName,
				Arguments = arguments,
				RedirectStandardOutput = true,
				CreateNoWindow = true,
			};
			var process = Process.Start(startInfo);
			if ( process == null ) {
				return string.Empty;
			}
			var output = process.StandardOutput.ReadToEnd();
			process.WaitForExit();
			return output;
		}

		public static string[] StartAndReadLines(string fileName, string arguments) {
			return StartAndReadOutput(fileName, arguments).Split('\n');
		}
	}
}
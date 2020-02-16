using System;
using System.Diagnostics;

namespace Core.Common.Utils {
	public static class ProgramRunner {
		public static (int, string) Start(string fileName, string arguments, string workDir) {
			var startInfo = new ProcessStartInfo {
				FileName               = fileName,
				Arguments              = arguments,
				WorkingDirectory       = workDir,
				UseShellExecute        = false,
				RedirectStandardOutput = true,
				RedirectStandardError  = true,
			};
			using ( var process = Process.Start(startInfo) ) {
				if ( process == null ) {
					throw new InvalidOperationException("Failed to start process!");
				}
				process.WaitForExit();
				var exitCode = process.ExitCode;
				var output   = process.StandardOutput.ReadToEnd();
				var error    = process.StandardError.ReadToEnd();
				var message  = $"Output:\n{output}\nError:\n{error}";
				return (exitCode, message);
			}
		}
	}
}
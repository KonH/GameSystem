namespace Core.Client.UnityClient.ExternalCompiler {
	public class CompileResult {
		public readonly string Path;
		public readonly int    ExitCode;
		public readonly string Output;

		public bool Success => (ExitCode == 0);

		public CompileResult(string path, int exitCode, string output) {
			Path     = path;
			ExitCode = exitCode;
			Output   = output;
		}
	}
}
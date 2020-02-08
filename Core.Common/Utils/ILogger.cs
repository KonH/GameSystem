namespace Core.Common.Utils {
	public interface ILogger<T> {
		void Log(LogLevel logLevel, string message);
		void LogTrace(string message) => Log(LogLevel.Trace, message);
		void LogWarning(string message) => Log(LogLevel.Warning, message);
		void LogError(string message) => Log(LogLevel.Error, message);
	}
}
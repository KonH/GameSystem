namespace Core.Common.Utils {
	public interface ILogger<T> {
		void Log(LogLevel logLevel, string message);
	}
}
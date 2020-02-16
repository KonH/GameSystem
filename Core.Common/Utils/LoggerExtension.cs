namespace Core.Common.Utils {
	public static class LoggerExtension {
		public static void LogTrace<T>(this ILogger<T> logger, string message) =>
			logger.Log(LogLevel.Trace, message);

		public static void LogWarning<T>(this ILogger<T> logger, string message) =>
			logger.Log(LogLevel.Warning, message);

		public static void LogError<T>(this ILogger<T> logger, string message) =>
			logger.Log(LogLevel.Error, message);
	}
}
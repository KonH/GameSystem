using System;

namespace Core.Common.Utils {
	public sealed class ConsoleLogger<T> : ILogger<T> {
		public void Log(LogLevel logLevel, string message) {
			var format = $"[{typeof(T).Name}] {logLevel.ToString()}: {message}";
			Console.WriteLine(format);
		}
	}
}
using System;
using Core.Common.Utils;

namespace Core.Common.Tests.Utils {
	public sealed class TestLogger<T> : ILogger<T> {
		public void Log(LogLevel logLevel, string message) {
			var format = $"[{typeof(T).Name}] {logLevel.ToString()}: {message}";
			Console.WriteLine(format);
		}
	}
}
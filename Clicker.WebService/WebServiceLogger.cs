using System;
using Microsoft.Extensions.Logging;
using LogLevel = Core.Common.Utils.LogLevel;

namespace Clicker.WebService {
	public sealed class WebServiceLogger<T> : Core.Common.Utils.ILogger<T> {
		readonly ILogger _logger;

		public WebServiceLogger(ILogger logger) {
			_logger = logger;
		}

		public void Log(LogLevel logLevel, string message) {
			switch ( logLevel ) {
				case LogLevel.Trace:
					_logger.LogTrace(message);
					break;
				case LogLevel.Warning:
					_logger.LogWarning(message);
					break;
				case LogLevel.Error:
					_logger.LogError(message);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
			}
		}
	}
}
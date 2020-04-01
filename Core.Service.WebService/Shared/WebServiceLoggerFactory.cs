using Microsoft.Extensions.Logging;
using ILoggerFactory = Core.Common.Utils.ILoggerFactory;

namespace Core.Service.WebService.Shared {
	public sealed class WebServiceLoggerFactory : ILoggerFactory {
		readonly Microsoft.Extensions.Logging.ILoggerFactory _internalFactory;

		public WebServiceLoggerFactory(Microsoft.Extensions.Logging.ILoggerFactory internalFactory) {
			_internalFactory = internalFactory;
		}

		public Core.Common.Utils.ILogger<T> Create<T>() {
			var internalLogger = _internalFactory.CreateLogger(typeof(T));
			return new WebServiceLogger<T>(internalLogger);
		}
	}
}
using System;

namespace Core.Common.Utils {
	public sealed class LoggerFactory {
		readonly Type _openType;

		public LoggerFactory(Type openType) {
			_openType = openType;
		}

		public ILogger<T> Create<T>() {
			var closedType = _openType.MakeGenericType(typeof(T));
			return (ILogger<T>)Activator.CreateInstance(closedType);
		}
	}
}
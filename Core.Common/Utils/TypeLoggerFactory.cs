using System;

namespace Core.Common.Utils {
	public sealed class TypeLoggerFactory : ILoggerFactory {
		readonly Type _openType;

		public TypeLoggerFactory(Type openType) {
			_openType = openType;
		}

		public ILogger<T> Create<T>() {
			var closedType = _openType.MakeGenericType(typeof(T));
			return (ILogger<T>) Activator.CreateInstance(closedType);
		}
	}
}
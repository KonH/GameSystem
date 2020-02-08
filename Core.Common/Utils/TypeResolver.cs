using System;
using System.Linq;
using System.Reflection;

namespace Core.Common.Utils {
	public static class TypeResolver {
		public static Type[] GetSubclasses<TBase>(Assembly assembly) {
			return assembly
				.GetTypes()
				.Where(t => t.IsSubclassOf(typeof(TBase)) || t.GetInterfaces().Any(i => i == typeof(TBase)))
				.ToArray();
		}
	}
}
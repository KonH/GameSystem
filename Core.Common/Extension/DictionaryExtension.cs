using System.Collections.Generic;

namespace Core.Common.Extension {
	public static class DictionaryExtension {
		public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) {
			dictionary.TryGetValue(key, out var value);
			return value;
		}
	}
}
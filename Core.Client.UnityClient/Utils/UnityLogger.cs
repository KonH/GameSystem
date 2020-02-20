using System;
using System.Collections.Generic;
using Core.Common.Utils;
using UnityEngine;

namespace Core.Client.UnityClient.Utils {
	public sealed class UnityLogger<T> : ILogger<T> {
		Dictionary<LogLevel, Action<string>> _impls = new Dictionary<LogLevel, Action<string>> {
			{ LogLevel.Error, Debug.LogError },
			{ LogLevel.Warning, Debug.LogWarning },
			{ LogLevel.Trace, Debug.Log },
		};

		public void Log(LogLevel logLevel, string message) {
			var format = $"[{typeof(T).Name}]: {message}";
			_impls[logLevel](format);
		}
	}
}
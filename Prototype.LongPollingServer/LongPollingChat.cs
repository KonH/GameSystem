using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Prototype.LongPollingServer {
	public sealed class LongPollingChat {
		readonly ILogger<LongPollingChat> _logger;

		readonly List<TaskCompletionSource<string>> _connections = new List<TaskCompletionSource<string>>();

		public LongPollingChat(ILogger<LongPollingChat> logger) {
			_logger = logger;
		}

		public void Write(string message) {
			TaskCompletionSource<string>[] currentConnections;
			lock ( _connections ) {
				currentConnections = _connections.ToArray();
			}
			_logger.LogInformation($"Write message '{message}' to {currentConnections.Length} connections.");
			foreach ( var connection in currentConnections ) {
				connection.SetResult(message);
			}
		}

		public async Task<(bool, string)> WaitForMessage() {
			_logger.LogInformation("Wait for messages.");
			var connection = new TaskCompletionSource<string>();
			lock ( _connections ) {
				_logger.LogInformation("Add new connection.");
				_connections.Add(connection);
			}
			var task  = connection.Task;
			var delay = Task.Delay(TimeSpan.FromSeconds(60));
			await Task.WhenAny(task, delay);
			lock ( _connections ) {
				_logger.LogInformation("Remove connection.");
				_connections.Remove(connection);
			}
			if ( task.IsCompleted ) {
				_logger.LogInformation($"New message '{task.Result}'.");
				return (true, task.Result);
			}
			_logger.LogInformation("No messages found.");
			return (false, string.Empty);
		}
	}
}
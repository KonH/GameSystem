using System.Threading;
using System.Threading.Tasks;
using Nuke.Common;
using Renci.SshNet;

namespace BuildPipeline {
	public static class ServiceTarget {
		public sealed class ExecutionContext {
			public readonly string Host;
			public readonly string UserName;
			public readonly string Password;

			public ExecutionContext(string host, string userName, string password) {
				Host     = host;
				UserName = userName;
				Password = password;
			}
		}

		public static void StartService(ExecutionContext context, string serviceName) {
			ExecuteCommand(context, $"sudo systemctl start {serviceName}");
		}

		public static void StopService(ExecutionContext context, string serviceName) {
			ExecuteCommand(context, $"sudo systemctl stop {serviceName}");
		}

		static void ExecuteCommand(ExecutionContext context, string commandText) {
			ExecuteCommandAsync(context, commandText).GetAwaiter().GetResult();
		}

		static async Task ExecuteCommandAsync(ExecutionContext context, string commandText) {
			var connection = new ConnectionInfo(
				context.Host,
				context.UserName,
				new PasswordAuthenticationMethod(context.UserName, context.Password));

			var client = new SshClient(connection);

			PerformExecution(client, commandText);

			await TryToDisconnect(client);
		}

		static void PerformExecution(SshClient client, string commandText) {
			Logger.Info("Initialized");
			client.Connect();
			Logger.Info("Connected");

			using var command = client.CreateCommand(commandText);

			var output = command.Execute();
			var result = command.Result;
			Logger.Info($"Command finished: '{output}', '{result}'");
		}

		static async Task TryToDisconnect(SshClient client) {
			// Workaround for issue: https://github.com/sshnet/SSH.NET/issues/355
			var tokenSource       = new CancellationTokenSource();
			var cancellationToken = tokenSource.Token;
			var timeout           = 1000;
			var task              = Task.Run(() => PerformDisconnect(client), cancellationToken);
			if ( await Task.WhenAny(task, Task.Delay(timeout, cancellationToken)) == task ) {
				await task;
			} else {
				Logger.Info("Timeout happens");
			}
		}

		static void PerformDisconnect(SshClient client) {
			Logger.Info("Disconnecting");
			client.Disconnect();
			Logger.Info("Disconnected");
			client.Dispose();
		}
	}
}
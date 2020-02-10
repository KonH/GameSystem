using System;
using Core.Client;
using Core.Common.Command;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Client.ConsoleClient {
	public sealed class StandaloneConsoleClient<TConfig, TState> : IConsoleClient<TConfig, TState>
		where TConfig : IConfig where TState : IState {
		readonly StandaloneClient<TConfig, TState> _client;

		public TState State => _client.State;

		public StandaloneConsoleClient(StandaloneClient<TConfig, TState> client) {
			_client = client;
		}

		public void Initialize() {}

		public void Apply(ICommand<TConfig, TState> command) {
			var result = _client.Apply(command);
			if ( result is BatchCommandResult<TConfig, TState>.BadCommand badResult ) {
				Console.WriteLine($"Command failed with '{badResult.Description}', rewind state");
				_client.Rewind();
			}
		}
	}
}
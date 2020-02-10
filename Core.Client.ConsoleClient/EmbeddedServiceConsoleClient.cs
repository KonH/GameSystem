using Core.Client;
using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Client.ConsoleClient {
	public sealed class EmbeddedServiceConsoleClient<TConfig, TState> : IConsoleClient<TConfig, TState>
		where TConfig : IConfig where TState : class, IState {
		readonly EmbeddedServiceClient<TConfig, TState> _client;

		public EmbeddedServiceConsoleClient(EmbeddedServiceClient<TConfig, TState> client) {
			_client = client;
		}

		public TState State => _client.State;

		public void Initialize() {
			_client.Initialize();
		}

		public void Apply(ICommand<TConfig, TState> command) {
			_client.Apply(command);
		}
	}
}
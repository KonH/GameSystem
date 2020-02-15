using System;
using Core.Common.CommandDependency;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Client.ConsoleClient.Setup {
	public static class SetupFactory<TConfig, TState>
		where TConfig : class, IConfig where TState : class, IState, new() {
		public static IClientSetup
			CreateByArguments(CommandQueue<TConfig, TState> queue, TConfig config, string[] args) {
			if ( args.Length == 0 ) {
				return new StandaloneClientSetup<TConfig, TState>(queue, config);
			}
			switch ( args[0] ) {
				case "embedded": return new EmbeddedClientSetup<TConfig, TState>(queue, config);
				case "web":      return new WebClientSetup<TConfig, TState>(queue, args[1]);
				default:         throw new InvalidOperationException("Unknown client type!");
			}
		}
	}
}
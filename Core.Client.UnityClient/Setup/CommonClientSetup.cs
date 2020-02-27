using Core.Client.Shared;
using Core.Client.UnityClient.DependencyInjection;
using Core.Client.UnityClient.Utils;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;
using Core.Common.Utils;

namespace Core.Client.UnityClient.Setup {
	abstract class CommonClientSetup<TConfig, TState> : IClientSetup
		where TConfig : IConfig where TState : IState {
		public virtual void Configure(ServiceProvider provider) {
			provider.AddService<ILoggerFactory>(new TypeLoggerFactory(typeof(UnityLogger<>)));
			provider.AddService(new CommandProvider<TConfig, TState>(typeof(TState).Assembly));
			provider.AddService<BatchCommandExecutor<TConfig, TState>>();
		}
	}
}
using System.Threading.Tasks;
using Core.Client.ConsoleClient.Utils;
using Core.Client.Shared;
using Core.Common.CommandDependency;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;
using Core.Common.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Client.ConsoleClient.Setup {
	abstract class CommonClientSetup<TConfig, TState> : IClientSetup
		where TConfig : IConfig where TState : IState {
		readonly CommandQueue<TConfig, TState> _queue;

		protected CommonClientSetup(CommandQueue<TConfig, TState> queue) {
			_queue = queue;
		}

		public virtual void Configure(ServiceCollection services) {
			services.AddSingleton<ILoggerFactory>(new TypeLoggerFactory(typeof(ConsoleLogger<>)));
			services.AddSingleton<ConsoleReader>();
			services.AddSingleton<ConsoleRunner<TConfig, TState>>();

			services.AddSingleton(_queue);
			services.AddSingleton(new CommandProvider<TConfig, TState>(typeof(TState).Assembly));
			services.AddSingleton<BatchCommandExecutor<TConfig, TState>>();
		}

		public abstract void Initialize(ServiceProvider provider);

		public virtual async Task Run(ServiceProvider provider) {
			var client = provider.GetRequiredService<ConsoleRunner<TConfig, TState>>();
			await client.Run();
		}
	}
}
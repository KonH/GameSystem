using Microsoft.Extensions.DependencyInjection;

namespace Core.Client.ConsoleClient.Setup {
	public interface IClientSetup {
		void Configure(ServiceCollection services);
		void Initialize(ServiceProvider provider);
		void Run(ServiceProvider provider);
	}
}
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Client.ConsoleClient.Setup {
	public interface IClientSetup {
		void Configure(ServiceCollection services);
		void Initialize(ServiceProvider provider);
		Task Run(ServiceProvider provider);
	}
}
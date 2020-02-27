using Core.Client.UnityClient.DependencyInjection;

namespace Core.Client.UnityClient.Setup {
	public interface IClientSetup {
		void Configure(ServiceProvider provider);
	}
}
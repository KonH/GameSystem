using Core.Client.UnityClient;
using Core.Client.UnityClient.DependencyInjection;
using Core.Client.UnityClient.Settings;
using Core.Common.CommandDependency;
using Core.Service.Queue;
using Core.Service.Shared;
using Idler.Common;
using Idler.Common.Command;
using Idler.Common.Config;
using Idler.Common.Queue;
using Idler.Common.Repository;
using Idler.Common.State;
using Idler.Common.Watcher;
using Idler.UnityClient.Repository;

namespace Idler.UnityClient {
	public static class IdlerEntryPoint {
		public static void Install() {
			var provider = ServiceProvider.Instance;
			provider.AddService<CommandQueue<GameConfig, GameState>, CommandQueue>();
			provider.AddServiceFromResources<ISettings, IdlerSettings>("Settings");
			provider.AddService<ITimeProvider, RealTimeProvider>();
			var mode = provider.GetService<ISettings>().Mode;
			var setup = SetupFactory<GameConfig, GameState>.Create(provider);
			setup.Configure(provider);
			switch ( mode ) {
				case ClientMode.Embedded: {
					provider.AddService<SharedStateRepository>();
					provider.AddService<ResourceUpdateWatcher>();
					var settings = provider.GetService<CommandScheduler<GameConfig, GameState>.Settings>();
					settings.AddWatcher(provider.GetService<ResourceUpdateWatcher>());
					var processor = provider.GetService<CommandProcessor<GameConfig, GameState>>();
					processor.Handle<SendSharedResourceCommand>(provider.CreateService<SendSharedResourceProcessor>().Handle);
					provider.AddService<ISharedStateProvider, EmbeddedSharedStateProvider>();
					break;
				}

				case ClientMode.Web: {
					provider.AddService<ISharedStateProvider, WebSharedStateProvider>();
					break;
				}
			}
		}
	}
}
using System;
using Core.Client.UnityClient;
using Core.Client.UnityClient.DependencyInjection;
using Core.Client.UnityClient.Settings;
using Core.Common.CommandDependency;
using Core.Service.Queue;
using Idler.Common;
using Idler.Common.Config;
using Idler.Common.State;
using Idler.Common.Watcher;
using UnityEngine;

namespace Idler.UnityClient {
	public static class IdlerEntryPoint {
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void Install() {
			var provider = ServiceProvider.Instance;
			provider.AddService<CommandQueue<GameConfig, GameState>, CommandQueue>();
			provider.AddServiceFromResources<ISettings, IdlerSettings>("Settings");
			var mode = provider.GetService<ISettings>().Mode;
			if ( mode == ClientMode.Standalone ) {
				throw new InvalidOperationException($"{nameof(ClientMode.Standalone)} is not supported!");
			}
			var setup = SetupFactory<GameConfig, GameState>.CreateWaitable(provider);
			setup.Configure(provider);
			switch ( mode ) {
				case ClientMode.Embedded: {
					provider.AddService<ResourceUpdateWatcher>();
					provider.GetService<CommandScheduler<GameConfig, GameState>.Settings>().AddWatcher(provider.GetService<ResourceUpdateWatcher>());
					break;
				}
			}
		}
	}
}
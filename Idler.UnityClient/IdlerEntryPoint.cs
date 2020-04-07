using Core.Client.UnityClient;
using Core.Client.UnityClient.DependencyInjection;
using Core.Client.UnityClient.Settings;
using Core.Common.CommandDependency;
using Idler.Common;
using Idler.Common.Config;
using Idler.Common.State;
using UnityEngine;

namespace Idler.UnityClient {
	public static class IdlerEntryPoint {
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void Install() {
			var provider = ServiceProvider.Instance;
			provider.AddService<CommandQueue<GameConfig, GameState>, CommandQueue>();
			provider.AddServiceFromResources<ISettings, IdlerSettings>("Settings");
			var setup = SetupFactory<GameConfig, GameState>.Create(provider);
			setup.Configure(provider);
		}
	}
}
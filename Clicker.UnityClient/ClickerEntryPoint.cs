using Clicker.Common;
using Clicker.Common.Config;
using Clicker.Common.State;
using Core.Client.UnityClient;
using Core.Client.UnityClient.DependencyInjection;
using Core.Client.UnityClient.Settings;
using Core.Common.CommandDependency;
using UnityEngine;

namespace Clicker.UnityClient {
	public static class ClickerEntryPoint {
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void Install() {
			var provider = ServiceProvider.Instance;
			provider.AddService<CommandQueue<GameConfig, GameState>, CommandQueue>();
			provider.AddServiceFromResources<ISettings, ClickerSettings>("Settings");
			var setup = SetupFactory<GameConfig, GameState>.Create(provider);
			setup.Configure(provider);
		}
	}
}
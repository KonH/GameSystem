using System;
using Core.Client.UnityClient.DependencyInjection;
using Core.Client.UnityClient.Settings;
using Core.Client.UnityClient.Setup;
using Core.Client.UnityClient.Utils;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Client.UnityClient {
	public static class SetupFactory<TConfig, TState>
		where TConfig : class, IConfig where TState : class, IState, new() {

		public static IClientSetup Create(ServiceProvider provider) {
			var settings = provider.GetService<ISettings>();
			switch ( settings.Mode ) {
				case ClientMode.Standalone: {
					provider.AddJsonFromResourcesAsService<TConfig>(settings.ConfigPath);
					return provider.CreateService<StandaloneClientSetup<TConfig, TState>>();
				}

				case ClientMode.Embedded: {
					provider.AddJsonFromResourcesAsService<TConfig>(settings.ConfigPath);
					return provider.CreateService<EmbeddedClientSetup<TConfig, TState>>();
				}

				case ClientMode.Web: {
					provider.AddService(new UnityWebRequestHandler.Settings(settings.BaseUrl));
					return provider.CreateService<WebClientSetup<TConfig, TState>>();
				}

				default:
					throw new ArgumentOutOfRangeException(
						nameof(settings.Mode), settings.Mode, "Unknown client type!");
			}
		}

		public static IClientSetup CreateWaitable(ServiceProvider provider) {
			var settings = provider.GetService<ISettings>();
			switch ( settings.Mode ) {
				case ClientMode.Embedded: {
					provider.AddJsonFromResourcesAsService<TConfig>(settings.ConfigPath);
					return provider.CreateService<EmbeddedWaitClientSetup<TConfig, TState>>();
				}

				default:
					throw new ArgumentOutOfRangeException(
						nameof(settings.Mode), settings.Mode, "Unknown client type!");
			}
		}
	}
}
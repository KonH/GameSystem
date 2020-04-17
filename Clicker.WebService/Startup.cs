using System;
using Clicker.Common;
using Clicker.Common.Config;
using Clicker.Common.State;
using Core.Common.CommandDependency;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.Threading;
using Core.Common.Utils;
using Core.Service.Extension;
using Core.Service.Queue;
using Core.Service.Repository;
using Core.Service.Repository.Config;
using Core.Service.Repository.State;
using Core.Service.UseCase.GetConfig;
using Core.Service.UseCase.GetState;
using Core.Service.UseCase.SendCommand;
using Core.Service.UseCase.WaitCommand;
using Core.Service.WebService.Configuration;
using Core.Service.WebService.Repository;
using Core.Service.WebService.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Clicker.WebService {
	public class Startup {
		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		GameConfig Config => new GameConfig {
			Version = new ConfigVersion("Config"),
			Resource = new ResourceConfig {
				ResourceByClick = 10
			},
			Upgrade = new UpgradeConfig {
				Levels = new[] {
					new UpgradeLevel {
						Cost  = 30,
						Power = 2.0
					},
					new UpgradeLevel {
						Cost  = 100,
						Power = 3.0
					},
					new UpgradeLevel {
						Cost  = 200,
						Power = 5.0
					}
				}
			}
		};

		public void ConfigureServices(IServiceCollection services) {
			var settings = Configuration.GetSection(typeof(Settings).Name).Get<Settings>();
			services.AddSingleton(settings);

			services
				.AddControllers()
				.AddNewtonsoftJson(opts => {
					opts.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
				});

			services.AddSingleton(JsonRepositoryDecoratorSettings.Create<GameConfig>());
			services.AddSingleton<IConfigRepository<GameConfig>, InMemoryConfigRepository<GameConfig>>();

			switch ( settings.RepositoryMode ) {
				case RepositoryMode.Embedded: {
					services.AddSingleton(JsonRepositoryDecoratorSettings.Create<GameState>());
					services.AddSingleton<IStateRepository<GameState>, InMemoryStateRepository<GameState>>();
					break;
				}

				case RepositoryMode.MongoDb: {
					services.AddSingleton<IStateRepository<GameState>, MongoStateRepository<GameState>>();
					break;
				}

				case RepositoryMode.CouchDb: {
					services.AddSingleton<IStateRepository<GameState>, CouchStateRepository<GameState>>();
					break;
				}
			}

			services.AddSingleton(new GetSingleConfigStrategy<GameConfig>.Settings(Config.Version));
			services.AddSingleton<IGetConfigStrategy<GameConfig>, GetSingleConfigStrategy<GameConfig>>();
			services.AddSingleton<GetConfigUseCase<GameConfig>>();

			services.AddSingleton<GetStateUseCase<GameState>>();

			services.AddSingleton<CommandWorkQueue<GameConfig, GameState>>();
			services.AddSingleton<CommandAwaiter<GameConfig, GameState>>();
			services.AddSingleton<CommandScheduler<GameConfig, GameState>>();
			services.AddSingleton<ITaskRunner, DefaultTaskRunner>();
			services.AddSingleton(new WaitCommandSettings {
				WaitTime = TimeSpan.FromSeconds(60)
			});
			services.AddSingleton<WaitCommandUseCase<GameConfig, GameState>>();

			services.AddSingleton<ILoggerFactory, WebServiceLoggerFactory>();
			services.AddSingleton<CommandQueue<GameConfig, GameState>, CommandQueue>();
			services.AddSingleton<CommandExecutor<GameConfig, GameState>>();
			services.AddSingleton<BatchCommandExecutor<GameConfig, GameState>>();
			services.AddSingleton<CommonWatcher<GameConfig, GameState>>();
			services.AddSingleton(sp => {
				var scheduleSettings = new CommandScheduler<GameConfig, GameState>.Settings();
				scheduleSettings.AddWatcher(sp.GetService<CommonWatcher<GameConfig, GameState>>());
				return scheduleSettings;
			});
			services.AddSingleton<SendCommandUseCase<GameConfig, GameState>>();

			services.AddHostedService<UpdateCommandSchedulerService<GameConfig, GameState>>();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
			if ( env.IsDevelopment() ) {
				app.UseDeveloperExceptionPage();
			}
			app.UseRouting();
			app.UseAuthorization();
			app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

			var configRepository = app.ApplicationServices.GetRequiredService<IConfigRepository<GameConfig>>();
			configRepository.Add(Config);
		}
	}
}
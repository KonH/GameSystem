using System;
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
using Core.Service.Shared;
using Core.Service.UseCase.GetConfig;
using Core.Service.UseCase.GetState;
using Core.Service.UseCase.SendCommand;
using Core.Service.UseCase.WaitCommand;
using Core.Service.WebService.Configuration;
using Core.Service.WebService.Repository;
using Core.Service.WebService.Shared;
using Idler.Common;
using Idler.Common.Command;
using Idler.Common.Config;
using Idler.Common.Queue;
using Idler.Common.Repository;
using Idler.Common.State;
using Idler.Common.Watcher;
using Idler.WebService.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Idler.WebService {
	public class Startup {
		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		GameConfig Config => new GameConfig {
			Version = new ConfigVersion("Config"),
			Resource = new ResourceConfig {
				ResourceByTick = 10,
				SharedCost = 100,
			},
			Time = new TimeConfig {
				TickInterval = 15
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
					services.AddSingleton<ISharedStateRepository, InMemorySharedStateRepository>();
					break;
				}

				case RepositoryMode.CouchDb: {
					services.AddSingleton<IStateRepository<GameState>, CouchStateRepository<GameState>>();
					services.AddSingleton<ISharedStateRepository, CouchSharedStateRepository>();
					break;
				}
			}

			services.AddSingleton(new GetSingleConfigStrategy<GameConfig>.Settings(Config.Version));
			services.AddSingleton<IGetConfigStrategy<GameConfig>, GetSingleConfigStrategy<GameConfig>>();
			services.AddSingleton<GetConfigUseCase<GameConfig>>();

			services.AddSingleton<GetStateUseCase<GameState>>();

			services.AddSingleton<ILoggerFactory, WebServiceLoggerFactory>();
			services.AddSingleton<CommandQueue<GameConfig, GameState>, CommandQueue>();
			services.AddSingleton<CommandExecutor<GameConfig, GameState>>();
			services.AddSingleton<BatchCommandExecutor<GameConfig, GameState>>();
			services.AddSingleton<SendCommandUseCase<GameConfig, GameState>>();

			services.AddSingleton(new WaitCommandSettings {
				WaitTime = TimeSpan.FromSeconds(60)
			});
			services.AddSingleton<WaitCommandUseCase<GameConfig, GameState>>();

			services.AddSingleton<ITimeProvider, RealTimeProvider>();
			services.AddSingleton<CommandWorkQueue<GameConfig, GameState>>();
			services.AddSingleton<CommandAwaiter<GameConfig, GameState>>();
			services.AddSingleton<CommandScheduler<GameConfig, GameState>>();
			services.AddSingleton<ITaskRunner, DefaultTaskRunner>();
			services.AddSingleton<CommonWatcher<GameConfig, GameState>>();
			services.AddSingleton<ResourceUpdateWatcher>();
			services.AddSingleton(sp => {
				var schedulerSettings = new CommandScheduler<GameConfig, GameState>.Settings();
				schedulerSettings.AddWatcher(sp.GetService<CommonWatcher<GameConfig, GameState>>());
				schedulerSettings.AddWatcher(sp.GetService<ResourceUpdateWatcher>());
				return schedulerSettings;
			});

			services.AddSingleton<SendSharedResourceProcessor>();
			services.AddSingleton(sp => {
				var processor = new CommandProcessor<GameConfig, GameState>(sp.GetService<ITaskRunner>());
				processor.Handle<SendSharedResourceCommand>(sp.GetService<SendSharedResourceProcessor>().Handle);
				return processor;
			});

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
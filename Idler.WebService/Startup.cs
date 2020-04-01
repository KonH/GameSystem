using Clicker.Common;
using Idler.Common.Config;
using Idler.Common.State;
using Core.Common.CommandDependency;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.Utils;
using Core.Service.Extension;
using Core.Service.Repository;
using Core.Service.Repository.Config;
using Core.Service.Repository.State;
using Core.Service.UseCase.GetConfig;
using Core.Service.UseCase.GetState;
using Core.Service.UseCase.UpdateState;
using Core.Service.WebService.Configuration;
using Core.Service.WebService.Repository;
using Core.Service.WebService.Shared;
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
				ResourceByTick = 10
			},
			Time = new TimeConfig {
				TickInterval = 30
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

			services.AddSingleton<ILoggerFactory, WebServiceLoggerFactory>();
			services.AddSingleton<CommandQueue<GameConfig, GameState>, CommandQueue>();
			services.AddSingleton<CommandExecutor<GameConfig, GameState>>();
			services.AddSingleton<BatchCommandExecutor<GameConfig, GameState>>();
			services.AddSingleton<UpdateStateUseCase<GameConfig, GameState>>();
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
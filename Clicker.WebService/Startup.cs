using Clicker.Common;
using Clicker.Common.Config;
using Clicker.Common.State;
using Core.Common.CommandDependency;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.Utils;
using Core.Service.Extension;
using Core.Service.Model;
using Core.Service.Repository;
using Core.Service.Repository.Config;
using Core.Service.Repository.State;
using Core.Service.UseCase.GetConfig;
using Core.Service.UseCase.GetState;
using Core.Service.UseCase.UpdateState;
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
			services
				.AddControllers()
				.AddNewtonsoftJson(opts => {
					opts.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
				});

			services.AddSingleton(JsonRepositoryDecoratorSettings.Create<GameConfig>());
			services.AddSingleton<IConfigRepository<GameConfig>, InMemoryConfigRepository<GameConfig>>();

			services.AddSingleton(JsonRepositoryDecoratorSettings.Create<GameState>());
			services.AddSingleton<IStateRepository<GameState>, InMemoryStateRepository<GameState>>();

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

			var stateRepository = app.ApplicationServices.GetRequiredService<IStateRepository<GameState>>();
			stateRepository.Add(new UserId("UserId"), new GameState());
		}
	}
}
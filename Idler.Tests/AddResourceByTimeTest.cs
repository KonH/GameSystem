using System;
using System.Threading.Tasks;
using Core.Common.CommandDependency;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;
using Core.Common.Utils;
using Core.Service;
using Core.Service.Model;
using Core.Service.Shared;
using Core.Service.UseCase.WaitCommand;
using Core.TestTools;
using Idler.Common.Command;
using Idler.Common.Config;
using Idler.Common.State;
using Idler.Common.Watcher;
using NUnit.Framework;

namespace Idler.Tests {
	public sealed class AddResourceByTimeTest {
		[Test]
		public async Task IsResourceNotAddedWithoutTicks() {
			var scheduler = GetScheduler(new FixedTimeProvider());
			var useCase   = GetUseCase(scheduler);
			var req       = GetRequest(StateRepository.ValidUserId, new StateVersion(0));

			var resp = await useCase.Handle(req);

			Assert.IsInstanceOf<WaitCommandResponse.NotFound>(resp);
		}

		[Test]
		public async Task IsResourceAddedByOneTick() {
			var time      = new FixedTimeProvider();
			var scheduler = GetScheduler(time);
			var useCase   = GetUseCase(scheduler);
			var req       = GetRequest(StateRepository.ValidUserId, new StateVersion(0));

			time.Advance(TimeSpan.FromSeconds(1));

			var resp = await useCase.Handle(req);

			Assert.IsInstanceOf<WaitCommandResponse.Updated<GameConfig, GameState>>(resp);
			var nextCommands = ((WaitCommandResponse.Updated<GameConfig, GameState>)resp).NextCommands;
			Assert.AreEqual(2, nextCommands.Count);
			Assert.IsInstanceOf<AddResourceCommand>(nextCommands[0]);
			Assert.AreEqual(10, ((AddResourceCommand)nextCommands[0]).Amount);
		}

		[Test]
		public async Task IsLastDateUpdated() {
			var time      = new FixedTimeProvider();
			var scheduler = GetScheduler(time);
			var useCase   = GetUseCase(scheduler);
			var req       = GetRequest(StateRepository.ValidUserId, new StateVersion(0));
			var startTime = time.UtcNow;

			time.Advance(TimeSpan.FromSeconds(1));

			var resp = await useCase.Handle(req);

			Assert.IsInstanceOf<WaitCommandResponse.Updated<GameConfig, GameState>>(resp);
			var nextCommands = ((WaitCommandResponse.Updated<GameConfig, GameState>)resp).NextCommands;
			Assert.AreEqual(2, nextCommands.Count);
			Assert.IsInstanceOf<UpdateLastDateCommand>(nextCommands[1]);
			Assert.AreEqual(startTime.ToUnixTimeSeconds() + 1, ((UpdateLastDateCommand)nextCommands[1]).LastDate.ToUnixTimeSeconds());
		}

		[Test]
		public async Task IsResourceAddedBySeveralTicks() {
			var time      = new FixedTimeProvider();
			var scheduler = GetScheduler(time);
			var useCase   = GetUseCase(scheduler);
			var req       = GetRequest(StateRepository.ValidUserId, new StateVersion(0));

			time.Advance(TimeSpan.FromSeconds(2));

			var resp = await useCase.Handle(req);

			Assert.IsInstanceOf<WaitCommandResponse.Updated<GameConfig, GameState>>(resp);
			var nextCommands = ((WaitCommandResponse.Updated<GameConfig, GameState>)resp).NextCommands;
			Assert.AreEqual(2, nextCommands.Count);
			Assert.IsInstanceOf<AddResourceCommand>(nextCommands[0]);
			Assert.AreEqual(20, ((AddResourceCommand)nextCommands[0]).Amount);
		}

		GameConfig GetConfig() =>
			new GameConfig {
				Resource = new ResourceConfig {
					ResourceByTick = 10
				},
				Time = new TimeConfig {
					TickInterval = 1
				}
			};

		CommandScheduler<GameConfig, GameState> GetScheduler(ITimeProvider timeProvider) {
			var settings = new CommandScheduler<GameConfig, GameState>.Settings();
			settings.AddWatcher(new ResourceUpdateWatcher(timeProvider));
			return new CommandScheduler<GameConfig, GameState>(settings);
		}

		WaitCommandUseCase<GameConfig, GameState> GetUseCase(CommandScheduler<GameConfig, GameState> scheduler) {
			var settings         = new WaitCommandSettings { WaitTime = TimeSpan.Zero };
			var stateRepository  = StateRepository<GameState>.Create();
			var configRepository = ConfigRepository<GameConfig>.Create(GetConfig());
			var loggerFactory    = new TypeLoggerFactory(typeof(ConsoleLogger<>));
			var queue            = new CommandQueue<GameConfig, GameState>();
			var commandExecutor  = new BatchCommandExecutor<GameConfig, GameState>(loggerFactory, new CommandExecutor<GameConfig, GameState>(), queue);
			return new WaitCommandUseCase<GameConfig, GameState>(settings, scheduler, stateRepository, configRepository, commandExecutor);
		}

		WaitCommandRequest GetRequest(UserId userId, StateVersion stateVersion) {
			return new WaitCommandRequest(userId, stateVersion, new ConfigVersion());
		}
	}
}
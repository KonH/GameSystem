using System;
using System.Threading.Tasks;
using Core.Common.CommandDependency;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;
using Core.Common.Threading;
using Core.Common.Utils;
using Core.Service.Model;
using Core.Service.Queue;
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
			var queue     = new CommandWorkQueue<GameConfig, GameState>();
			var awaiter   = new CommandAwaiter<GameConfig, GameState>(queue);
			var scheduler = GetScheduler(new FixedTimeProvider(), queue);
			var useCase   = GetUseCase(awaiter);
			var req       = GetRequest(StateRepository.ValidUserId, new StateVersion(0));

			var task = useCase.Handle(req);
			await scheduler.Update();
			var resp = await task;

			Assert.IsInstanceOf<WaitCommandResponse.NotFound>(resp);
		}

		[Test]
		public async Task IsResourceAddedByOneTick() {
			var time      = new FixedTimeProvider();
			var queue     = new CommandWorkQueue<GameConfig, GameState>();
			var awaiter   = new CommandAwaiter<GameConfig, GameState>(queue);
			var scheduler = GetScheduler(time, queue);
			var useCase   = GetUseCase(awaiter);
			var req       = GetRequest(StateRepository.ValidUserId, new StateVersion(0));

			awaiter.OnWait += () => scheduler.Update();
			time.Advance(TimeSpan.FromSeconds(1));

			var task = useCase.Handle(req);
			var resp = await task;

			Assert.IsInstanceOf<WaitCommandResponse.Updated<GameConfig, GameState>>(resp);
			var nextCommands = ((WaitCommandResponse.Updated<GameConfig, GameState>)resp).NextCommands;
			Assert.AreEqual(2, nextCommands.Length);
			Assert.IsInstanceOf<AddResourceCommand>(nextCommands[0]);
			Assert.AreEqual(10, ((AddResourceCommand)nextCommands[0]).Amount);
		}

		[Test]
		public async Task IsLastDateUpdated() {
			var time      = new FixedTimeProvider();
			var queue     = new CommandWorkQueue<GameConfig, GameState>();
			var awaiter   = new CommandAwaiter<GameConfig, GameState>(queue);
			var scheduler = GetScheduler(time, queue);
			var useCase   = GetUseCase(awaiter);
			var req       = GetRequest(StateRepository.ValidUserId, new StateVersion(0));
			var startTime = time.UtcNow;

			awaiter.OnWait += () => scheduler.Update();
			time.Advance(TimeSpan.FromSeconds(1));

			var task = useCase.Handle(req);
			var resp = await task;

			Assert.IsInstanceOf<WaitCommandResponse.Updated<GameConfig, GameState>>(resp);
			var nextCommands = ((WaitCommandResponse.Updated<GameConfig, GameState>)resp).NextCommands;
			Assert.AreEqual(2, nextCommands.Length);
			Assert.IsInstanceOf<UpdateLastDateCommand>(nextCommands[1]);
			Assert.AreEqual(startTime.ToUnixTimeSeconds() + 1, ((UpdateLastDateCommand)nextCommands[1]).LastDate.ToUnixTimeSeconds());
		}

		[Test]
		public async Task IsResourceAddedBySeveralTicks() {
			var time      = new FixedTimeProvider();
			var queue     = new CommandWorkQueue<GameConfig, GameState>();
			var awaiter   = new CommandAwaiter<GameConfig, GameState>(queue);
			var scheduler = GetScheduler(time, queue);
			var useCase   = GetUseCase(awaiter);
			var req       = GetRequest(StateRepository.ValidUserId, new StateVersion(0));

			awaiter.OnWait += () => scheduler.Update();
			time.Advance(TimeSpan.FromSeconds(2));

			var task = useCase.Handle(req);
			var resp = await task;

			Assert.IsInstanceOf<WaitCommandResponse.Updated<GameConfig, GameState>>(resp);
			var nextCommands = ((WaitCommandResponse.Updated<GameConfig, GameState>)resp).NextCommands;
			Assert.AreEqual(2, nextCommands.Length);
			Assert.IsInstanceOf<AddResourceCommand>(nextCommands[0]);
			Assert.AreEqual(20, ((AddResourceCommand)nextCommands[0]).Amount);
		}

		GameConfig GetConfig() =>
			new GameConfig {
				Resource = new ResourceConfig {
					ResourceByTick = 10,
					SharedCost = 100,
				},
				Time = new TimeConfig {
					TickInterval = 1
				}
			};

		CommandScheduler<GameConfig, GameState> GetScheduler(ITimeProvider timeProvider, CommandWorkQueue<GameConfig, GameState> queue) {
			var settings = new CommandScheduler<GameConfig, GameState>.Settings();
			settings.AddWatcher(new ResourceUpdateWatcher(timeProvider));
			return new CommandScheduler<GameConfig, GameState>(settings, queue, StateRepository<GameState>.Create(), CreateExecutor());
		}

		WaitCommandUseCase<GameConfig, GameState> GetUseCase(CommandAwaiter<GameConfig, GameState> awaiter) {
			var settings         = new WaitCommandSettings { WaitTime = TimeSpan.Zero };
			var stateRepository  = StateRepository<GameState>.Create();
			var configRepository = ConfigRepository<GameConfig>.Create(GetConfig());
			return new WaitCommandUseCase<GameConfig, GameState>(settings, awaiter, stateRepository, configRepository, new DefaultTaskRunner());
		}

		WaitCommandRequest GetRequest(UserId userId, StateVersion stateVersion) {
			return new WaitCommandRequest(userId, stateVersion, new ConfigVersion());
		}

		public static BatchCommandExecutor<GameConfig, GameState> CreateExecutor() {
			var loggerFactory = new TypeLoggerFactory(typeof(ConsoleLogger<>));
			var queue         = new CommandQueue<GameConfig, GameState>();
			return new BatchCommandExecutor<GameConfig, GameState>(loggerFactory, new CommandExecutor<GameConfig, GameState>(loggerFactory), queue);
		}
	}
}
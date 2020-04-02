using System;
using System.Threading.Tasks;
using Core.Common.Command;
using Core.Common.CommandDependency;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;
using Core.Common.Utils;
using Core.Service.Model;
using Core.Service.UseCase.WaitCommand;
using Core.TestTools;
using NUnit.Framework;

namespace Core.Service.Tests.UseCase {
	public sealed class WaitCommandUseCaseTest {
		sealed class Config : IConfig {
			public ConfigVersion Version { get; set; } = new ConfigVersion();
		}

		sealed class State : IState {
			public StateVersion Version { get; set; } = new StateVersion();
		}

		sealed class OkCommand : ICommand<Config, State> {
			public CommandResult Apply(Config config, State state) {
				return CommandResult.Ok();
			}
		}

		sealed class Watcher : IUpdateWatcher<Config, State> {
			public bool IsNotified { get; private set; }

			public void OnCommandRequest(Config config, State state, UserId userId, CommandScheduler<Config, State> scheduler) {
				IsNotified = true;
			}
		}

		[Test]
		public async Task IsCommandNotFound() {
			var useCase = GetUseCase(GetScheduler());
			var req     = GetRequest(StateRepository.ValidUserId, new StateVersion(0));

			var resp = await useCase.Handle(req);

			Assert.IsInstanceOf<WaitCommandResponse.NotFound>(resp);
		}

		[Test]
		public async Task IsCommandFound() {
			var scheduler = GetScheduler();
			var useCase   = GetUseCase(scheduler);
			var req       = GetRequest(StateRepository.ValidUserId, new StateVersion(0));

			scheduler.AddCommand(StateRepository.ValidUserId, new OkCommand());

			var resp = await useCase.Handle(req);

			Assert.IsInstanceOf<WaitCommandResponse.Updated<Config, State>>(resp);
			var nextCommands = ((WaitCommandResponse.Updated<Config, State>)resp).NextCommands;
			Assert.AreEqual(1, nextCommands.Count);
			Assert.IsInstanceOf<OkCommand>(nextCommands[0]);
		}

		[Test]
		public async Task IsSeveralCommandsFound() {
			var scheduler = GetScheduler();
			var useCase   = GetUseCase(scheduler);
			var req       = GetRequest(StateRepository.ValidUserId, new StateVersion(0));

			scheduler.AddCommand(StateRepository.ValidUserId, new OkCommand());
			scheduler.AddCommand(StateRepository.ValidUserId, new OkCommand());

			var resp = await useCase.Handle(req);

			Assert.IsInstanceOf<WaitCommandResponse.Updated<Config, State>>(resp);
			var nextCommands = ((WaitCommandResponse.Updated<Config, State>)resp).NextCommands;
			Assert.AreEqual(2, nextCommands.Count);
		}

		[Test]
		public async Task IsStateOutdated() {
			var useCase = GetUseCase(GetScheduler());
			var req     = GetRequest(StateRepository.ValidUserId, new StateVersion(-1));

			var resp = await useCase.Handle(req);

			Assert.IsInstanceOf<WaitCommandResponse.Outdated>(resp);
		}

		[Test]
		public async Task IsWatcherNotified() {
			var settings = new CommandScheduler<Config, State>.Settings();
			var watcher  = new Watcher();
			settings.AddWatcher(watcher);
			var scheduler = new CommandScheduler<Config, State>(settings);
			var useCase   = GetUseCase(scheduler);
			var req       = GetRequest(StateRepository.ValidUserId, new StateVersion(0));

			await useCase.Handle(req);

			Assert.True(watcher.IsNotified);
		}

		CommandScheduler<Config, State> GetScheduler() =>
			new CommandScheduler<Config, State>(new CommandScheduler<Config, State>.Settings());

		WaitCommandUseCase<Config, State> GetUseCase(CommandScheduler<Config, State> scheduler) {
			var settings         = new WaitCommandSettings { WaitTime = TimeSpan.Zero };
			var stateRepository  = StateRepository<State>.Create();
			var configRepository = ConfigRepository<Config>.Create(new Config());
			var loggerFactory    = new TypeLoggerFactory(typeof(ConsoleLogger<>));
			var queue            = new CommandQueue<Config, State>();
			var commandExecutor  = new BatchCommandExecutor<Config, State>(loggerFactory, new CommandExecutor<Config, State>(), queue);
			return new WaitCommandUseCase<Config, State>(settings, scheduler, stateRepository, configRepository, commandExecutor);
		}

		WaitCommandRequest GetRequest(UserId userId, StateVersion stateVersion) {
			return new WaitCommandRequest(userId, stateVersion, new ConfigVersion());
		}
	}
}
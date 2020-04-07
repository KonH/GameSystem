using System;
using System.Threading.Tasks;
using Core.Common.Command;
using Core.Common.CommandDependency;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;
using Core.Common.Threading;
using Core.Common.Utils;
using Core.Service.Model;
using Core.Service.Queue;
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

		sealed class OkWatcher : IUpdateWatcher<Config, State> {
			public void TryAddCommands(UserId userId, Config config, State state, CommandSet<Config, State> commands) {
				commands.Add(new OkCommand());
			}
		}

		[Test]
		public async Task IsCommandNotFound() {
			var queue   = new CommandWorkQueue<Config, State>();
			var awaiter = new CommandAwaiter<Config, State>(queue);
			var useCase = GetUseCase(awaiter);
			var req     = GetRequest(StateRepository.ValidUserId, new StateVersion(0));

			var resp = await useCase.Handle(req);

			Assert.IsInstanceOf<WaitCommandResponse.NotFound>(resp);
		}

		[Test]
		public async Task IsCommandFound() {
			var settings = new CommandScheduler<Config, State>.Settings();
			var watcher = new OkWatcher();
			settings.AddWatcher(watcher);
			var queue     = new CommandWorkQueue<Config, State>();
			var scheduler = new CommandScheduler<Config, State>(settings, queue);
			var awaiter   = new CommandAwaiter<Config, State>(queue);
			var useCase   = GetUseCase(awaiter);
			var req       = GetRequest(StateRepository.ValidUserId, new StateVersion(0));

			awaiter.OnWait += () => scheduler.Update();

			var task = useCase.Handle(req);
			var resp = await task;

			Assert.IsInstanceOf<WaitCommandResponse.Updated<Config, State>>(resp);
			var nextCommands = ((WaitCommandResponse.Updated<Config, State>)resp).NextCommands;
			Assert.AreEqual(1, nextCommands.Count);
			Assert.IsInstanceOf<OkCommand>(nextCommands[0]);
		}

		[Test]
		public async Task IsStateOutdated() {
			var queue   = new CommandWorkQueue<Config, State>();
			var awaiter = new CommandAwaiter<Config, State>(queue);
			var useCase = GetUseCase(awaiter);
			var req     = GetRequest(StateRepository.ValidUserId, new StateVersion(-1));

			var resp = await useCase.Handle(req);

			Assert.IsInstanceOf<WaitCommandResponse.Outdated>(resp);
		}

		WaitCommandUseCase<Config, State> GetUseCase(CommandAwaiter<Config, State> awaiter) {
			var settings         = new WaitCommandSettings { WaitTime = TimeSpan.Zero };
			var stateRepository  = StateRepository<State>.Create();
			var configRepository = ConfigRepository<Config>.Create(new Config());
			var loggerFactory    = new TypeLoggerFactory(typeof(ConsoleLogger<>));
			var queue            = new CommandQueue<Config, State>();
			var commandExecutor  = new BatchCommandExecutor<Config, State>(loggerFactory, new CommandExecutor<Config, State>(), queue);
			return new WaitCommandUseCase<Config, State>(settings, awaiter, stateRepository, configRepository, commandExecutor, new DefaultTaskRunner());
		}

		WaitCommandRequest GetRequest(UserId userId, StateVersion stateVersion) {
			return new WaitCommandRequest(userId, stateVersion, new ConfigVersion());
		}
	}
}
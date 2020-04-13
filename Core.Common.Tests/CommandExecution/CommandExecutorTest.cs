using System.Threading.Tasks;
using Core.Common.Command;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;
using Core.Common.Utils;
using NUnit.Framework;

namespace Core.Common.Tests.CommandExecution {
	public sealed class CommandExecutorTest {
		sealed class State : IState {
			public StateVersion Version { get; set; } = new StateVersion();
		}

		sealed class Config : IConfig {
			public ConfigVersion Version { get; set; }
		}

		sealed class OkCommand : ICommand<Config, State> {
			public CommandResult Apply(Config config, State state) {
				return CommandResult.Ok();
			}
		}

		sealed class BadCommand : ICommand<Config, State> {
			public CommandResult Apply(Config config, State state) {
				return CommandResult.BadCommand("");
			}
		}

		[Test]
		public async Task IsCommandApplied() {
			var executor = CreateExecutor();

			var result = await executor.Apply(new Config(), new State(), new OkCommand());

			Assert.IsInstanceOf<CommandResult.OkResult>(result);
		}

		[Test]
		public async Task IsVersionIncremented() {
			var executor = CreateExecutor();
			var state    = new State();

			await executor.Apply(new Config(), state, new OkCommand());

			Assert.AreEqual(1, state.Version.Value);
		}

		[Test]
		public async Task IsCommandNotAppliedIfInvalid() {
			var executor = CreateExecutor();

			var result = await executor.Apply(new Config(), new State(), new BadCommand());

			Assert.IsInstanceOf<CommandResult.BadCommandResult>(result);
		}

		[Test]
		public async Task IsCommandNotAppliedIfNull() {
			var executor = CreateExecutor();

			var result = await executor.Apply(new Config(), new State(), (OkCommand) null);

			Assert.IsInstanceOf<CommandResult.BadCommandResult>(result);
		}

		[Test]
		public async Task IsCommandNotAppliedIfStateNull() {
			var executor = CreateExecutor();

			var result = await executor.Apply(new Config(), null, (OkCommand) null);

			Assert.IsInstanceOf<CommandResult.BadCommandResult>(result);
		}

		[Test]
		public async Task IsCommandNotAppliedIfConfigNull() {
			var executor = CreateExecutor();

			var result = await executor.Apply(null, new State(), (OkCommand) null);

			Assert.IsInstanceOf<CommandResult.BadCommandResult>(result);
		}

		CommandExecutor<Config, State> CreateExecutor() => new CommandExecutor<Config, State>(new TypeLoggerFactory(typeof(ConsoleLogger<>)));
	}
}
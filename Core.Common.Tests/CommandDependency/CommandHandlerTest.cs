using System.Linq;
using Core.Common.Command;
using Core.Common.CommandDependency;
using Core.Common.Config;
using Core.Common.State;
using NUnit.Framework;

namespace Core.Common.Tests.CommandDependency {
	public sealed class CommandHandlerTest {
		sealed class State : IState {
			public StateVersion Version { get; set; }
		}

		sealed class Config : IConfig {
			public ConfigVersion Version { get; set; }
		}

		sealed class FirstCommand : ICommand<Config, State> {
			public bool Value { get; set; }

			public FirstCommand() { }

			public FirstCommand(bool value) {
				Value = value;
			}

			public CommandResult Apply(Config config, State state) {
				return CommandResult.Ok();
			}
		}

		sealed class SecondCommand : ICommand<Config, State> {
			public CommandResult Apply(Config config, State state) {
				return CommandResult.Ok();
			}
		}

		[Test]
		public void ResultIsEmptyIfNoDependencies() {
			var handler = CreateHandler(new CommandQueue<Config, State>());

			var result = handler.GetDependentCommands(new FirstCommand());

			Assert.IsEmpty(result);
		}

		[Test]
		public void IsDependencyFound() {
			var handler = CreateHandler(new CommandQueue<Config, State>()
				.AddDependency((FirstCommand c) => new SecondCommand()));

			var result = handler.GetDependentCommands(new FirstCommand());

			Assert.NotNull(result.FirstOrDefault(c => c is SecondCommand));
		}

		[Test]
		public void IsMultipleDependencyFound() {
			var handler = CreateHandler(new CommandQueue<Config, State>()
				.AddDependency((FirstCommand c) => new SecondCommand())
				.AddDependency((FirstCommand c) => new SecondCommand()));

			var result = handler.GetDependentCommands(new FirstCommand());

			Assert.AreEqual(2, result.Count);
		}

		[Test]
		public void IsConditionalDependencyFoundIfSatisfied() {
			var handler = CreateHandler(new CommandQueue<Config, State>()
				.AddDependency((FirstCommand c) => c.Value, c => new SecondCommand()));

			var result = handler.GetDependentCommands(new FirstCommand(true));

			Assert.AreEqual(1, result.Count);
		}

		[Test]
		public void IsConditionalDependencyNotFoundIfNotSatisfied() {
			var handler = CreateHandler(new CommandQueue<Config, State>()
				.AddDependency((FirstCommand c) => c.Value, c => new SecondCommand()));

			var result = handler.GetDependentCommands(new FirstCommand(false));

			Assert.IsEmpty(result);
		}

		CommandHandler<Config, State> CreateHandler(CommandQueue<Config, State> queue) {
			return new CommandHandler<Config, State>(queue);
		}
	}
}
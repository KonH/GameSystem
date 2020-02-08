using System.Collections.Generic;
using Core.Common.Command;
using Core.Common.CommandDependency;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;
using Core.Common.Utils;
using NUnit.Framework;

namespace Core.Common.Tests.CommandExecution {
	public sealed class BatchBatchCommandExecutorTest {
		sealed class State : IState {
			public StateVersion Version { get; set; } = new StateVersion();

			public List<string> AppliedCommands { get; set; } = new List<string>();
		}

		sealed class Config : IConfig {
			public ConfigVersion Version { get; set; }
		}

		sealed class OkCommand : ICommand<Config, State> {
			public CommandResult Apply(Config config, State state) {
				state.AppliedCommands.Add("Main");
				return CommandResult.Ok();
			}
		}

		sealed class DependencyCommand : ICommand<Config, State> {
			public string Name { get; set; }

			public DependencyCommand() { }

			public DependencyCommand(string name) {
				Name = name;
			}

			public CommandResult Apply(Config config, State state) {
				state.AppliedCommands.Add(Name);
				return CommandResult.Ok();
			}

			public override string ToString() {
				return $"{nameof(DependencyCommand)}: '{Name}'";
			}
		}

		sealed class BadCommand : ICommand<Config, State> {
			public CommandResult Apply(Config config, State state) {
				return CommandResult.BadCommand("");
			}
		}

		[Test]
		public void IsMainCommandApplied() {
			var executor = CreateExecutor(new CommandQueue<Config, State>()
				.AddDependency((OkCommand c) => new DependencyCommand()));
			var state = new State();

			executor.Apply(new Config(), state, new OkCommand());

			Assert.Contains("Main", state.AppliedCommands);
		}

		[Test]
		public void IsBadResultIfMainCommandInvalid() {
			var executor = CreateExecutor(new CommandQueue<Config, State>()
				.AddDependency((OkCommand c) => new DependencyCommand()));
			var state = new State();

			var result = executor.Apply(new Config(), state, new BadCommand());

			Assert.IsInstanceOf<BatchCommandResult<Config, State>.BadCommandResult>(result);
		}

		[Test]
		public void IsDependencyApplied() {
			var executor = CreateExecutor(new CommandQueue<Config, State>()
				.AddDependency((OkCommand c) => new DependencyCommand("Dependency")));
			var state = new State();

			var result = executor.Apply(new Config(), state, new OkCommand());

			Assert.IsInstanceOf<BatchCommandResult<Config, State>.OkResult>(result);
			Assert.AreEqual(
				state.AppliedCommands.Count - 1,
				((BatchCommandResult<Config, State>.OkResult) result).NextCommands.Count);

			Assert.Contains("Dependency", state.AppliedCommands);
		}

		[Test]
		public void IsDependencyAppliedAfterMainCommand() {
			var executor = CreateExecutor(new CommandQueue<Config, State>()
				.AddDependency((OkCommand c) => new DependencyCommand("Dependency")));
			var state = new State();

			var result = executor.Apply(new Config(), state, new OkCommand());

			Assert.IsInstanceOf<BatchCommandResult<Config, State>.OkResult>(result);
			Assert.AreEqual(
				state.AppliedCommands.Count - 1,
				((BatchCommandResult<Config, State>.OkResult) result).NextCommands.Count);

			Assert.AreEqual("Main", state.AppliedCommands[0]);
			Assert.AreEqual("Dependency", state.AppliedCommands[1]);
		}

		[Test]
		public void IsDependenciesAppliedInCorrectOrder() {
			var executor = CreateExecutor(new CommandQueue<Config, State>()
				.AddDependency((OkCommand c) => new DependencyCommand("Dependency1"))
				.AddDependency(
					(DependencyCommand c) => c.Name == "Dependency1",
					c => new DependencyCommand("Dependency1_1"))
				.AddDependency((OkCommand c) => new DependencyCommand("Dependency2"))
				.AddDependency(
					(DependencyCommand c) => c.Name == "Dependency2",
					c => new DependencyCommand("Dependency2_1"))
				.AddDependency(
					(DependencyCommand c) => c.Name == "Dependency2_1",
					c => new DependencyCommand("Dependency2_2"))
				.AddDependency((OkCommand c) => new DependencyCommand("Dependency3")));
			var state = new State();

			var result = executor.Apply(new Config(), state, new OkCommand());

			Assert.IsInstanceOf<BatchCommandResult<Config, State>.OkResult>(result);
			Assert.AreEqual(
				state.AppliedCommands.Count - 1,
				((BatchCommandResult<Config, State>.OkResult) result).NextCommands.Count);

			Assert.AreEqual("Main", state.AppliedCommands[0]);
			Assert.AreEqual("Dependency1", state.AppliedCommands[1]);
			Assert.AreEqual("Dependency1_1", state.AppliedCommands[2]);
			Assert.AreEqual("Dependency2", state.AppliedCommands[3]);
			Assert.AreEqual("Dependency2_1", state.AppliedCommands[4]);
			Assert.AreEqual("Dependency2_2", state.AppliedCommands[5]);
			Assert.AreEqual("Dependency3", state.AppliedCommands[6]);
		}

		[Test]
		public void IsVersionIncrementedForMainCommand() {
			var executor = CreateExecutor(new CommandQueue<Config, State>());
			var state = new State();

			executor.Apply(new Config(), state, new OkCommand());

			Assert.AreEqual(1, state.Version.Value);
		}

		[Test]
		public void IsVersionIncrementedForDependency() {
			var executor = CreateExecutor(new CommandQueue<Config, State>()
				.AddDependency((OkCommand c) => new DependencyCommand()));
			var state = new State();

			executor.Apply(new Config(), state, new OkCommand());

			Assert.AreEqual(2, state.Version.Value);
		}

		[Test]
		public void IsVersionNotIncrementedIfDependencyFailed() {
			var executor = CreateExecutor(new CommandQueue<Config, State>()
				.AddDependency((OkCommand c) => new BadCommand()));
			var state = new State();

			executor.Apply(new Config(), state, new OkCommand());

			Assert.AreEqual(1, state.Version.Value);
		}

		[Test]
		public void IsBadResultIfDependencyFailed() {
			var executor = CreateExecutor(new CommandQueue<Config, State>()
				.AddDependency((OkCommand c) => new BadCommand()));
			var state = new State();

			var result = executor.Apply(new Config(), state, new OkCommand());

			Assert.IsInstanceOf<BatchCommandResult<Config, State>.BadCommandResult>(result);
		}

		BatchCommandExecutor<Config, State> CreateExecutor(CommandQueue<Config, State> queue) {
			var logger = new ConsoleLogger<BatchCommandExecutor<Config, State>>();
			return new BatchCommandExecutor<Config, State>(logger, queue);
		}
	}
}
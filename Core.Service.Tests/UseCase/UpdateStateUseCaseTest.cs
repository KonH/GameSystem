using Core.Common.Command;
using Core.Common.CommandDependency;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;
using Core.Common.Utils;
using Core.Service.Model;
using Core.Service.UseCase.UpdateState;
using NUnit.Framework;

namespace Core.Service.Tests.UseCase {
	public sealed class UpdateStateUseCaseTest {
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

		sealed class BadCommand : ICommand<Config, State> {
			public CommandResult Apply(Config config, State state) {
				return CommandResult.BadCommand("");
			}
		}

		[Test]
		public void IsStateUpdated() {
			var useCase = GetUseCase();
			var req     = GetRequest(StateRepository.ValidUserId, new StateVersion(0));

			var resp = useCase.Handle(req);

			Assert.IsInstanceOf<UpdateStateResponse.Updated<Config, State>>(resp);
		}

		[Test]
		public void IsStateRejected() {
			var useCase = GetUseCase();
			var req     = GetRequest(StateRepository.ValidUserId, new StateVersion(0), new BadCommand());

			var resp = useCase.Handle(req);

			Assert.IsInstanceOf<UpdateStateResponse.Rejected>(resp);
		}

		[Test]
		public void IsStateNotFound() {
			var useCase = GetUseCase();
			var req     = GetRequest(new UserId("InvalidUserId"), new StateVersion(0));

			var resp = useCase.Handle(req);

			Assert.IsInstanceOf<UpdateStateResponse.NotFound>(resp);
		}

		[Test]
		public void IsStateOutdated() {
			var useCase = GetUseCase();
			var req     = GetRequest(StateRepository.ValidUserId, new StateVersion(-1));

			var resp = useCase.Handle(req);

			Assert.IsInstanceOf<UpdateStateResponse.Outdated>(resp);
		}

		UpdateStateUseCase<Config, State> GetUseCase() {
			var stateRepository  = StateRepository<State>.Create();
			var configRepository = ConfigRepository<Config>.Create(new Config());
			var loggerFactory    = new TypeLoggerFactory(typeof(ConsoleLogger<>));
			var queue            = new CommandQueue<Config, State>();
			var commandExecutor  = new BatchCommandExecutor<Config, State>(loggerFactory, queue);
			return new UpdateStateUseCase<Config, State>(stateRepository, configRepository, commandExecutor);
		}

		UpdateStateRequest<Config, State> GetRequest(UserId userId, StateVersion stateVersion) {
			return GetRequest(userId, stateVersion, new OkCommand());
		}

		UpdateStateRequest<Config, State> GetRequest(
			UserId userId, StateVersion stateVersion, ICommand<Config, State> command) {
			var configVersion = new ConfigVersion();
			return new UpdateStateRequest<Config, State>(userId, stateVersion, configVersion, command);
		}
	}
}
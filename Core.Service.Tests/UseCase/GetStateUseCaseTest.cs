using Core.Common.State;
using Core.Service.Model;
using Core.Service.UseCase.GetState;
using NUnit.Framework;

namespace Core.Service.Tests.UseCase {
	public sealed class GetStateUseCaseTest {
		sealed class State : IState {
			public StateVersion Version { get; set; } = new StateVersion();
		}

		[Test]
		public void IsStateFound() {
			var useCase = GetUseCase();
			var req     = GetRequest(StateRepository.ValidUserId);

			var resp = useCase.Handle(req);

			Assert.IsInstanceOf<GetStateResponse<State>.Found>(resp);
			Assert.NotNull(((GetStateResponse<State>.Found) resp).State);
		}

		[Test]
		public void IsStateNotFound() {
			var useCase = GetUseCase();
			var req     = GetRequest(new UserId("InvalidUserId"));

			var resp = useCase.Handle(req);

			Assert.IsInstanceOf<GetStateResponse<State>.NotFound>(resp);
		}

		GetStateUseCase<State> GetUseCase() {
			var repository = StateRepository<State>.Create();
			return new GetStateUseCase<State>(repository);
		}

		GetStateRequest GetRequest(UserId userId) {
			return new GetStateRequest(userId);
		}
	}
}
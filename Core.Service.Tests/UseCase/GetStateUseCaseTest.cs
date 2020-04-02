using System.Threading.Tasks;
using Core.Common.State;
using Core.Service.Model;
using Core.Service.UseCase.GetState;
using Core.TestTools;
using NUnit.Framework;

namespace Core.Service.Tests.UseCase {
	public sealed class GetStateUseCaseTest {
		sealed class State : IState {
			public StateVersion Version { get; set; } = new StateVersion();
		}

		[Test]
		public async Task IsStateFound() {
			var useCase = GetUseCase();
			var req     = GetRequest(StateRepository.ValidUserId);

			var resp = await useCase.Handle(req);

			Assert.IsInstanceOf<GetStateResponse.Found<State>>(resp);
			Assert.NotNull(((GetStateResponse.Found<State>) resp).State);
		}

		[Test]
		public async Task IsStateCreatedIfNotFound() {
			var useCase = GetUseCase();
			var req     = GetRequest(new UserId("InvalidUserId"));

			var resp = await useCase.Handle(req);

			Assert.IsInstanceOf<GetStateResponse.Found<State>>(resp);
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
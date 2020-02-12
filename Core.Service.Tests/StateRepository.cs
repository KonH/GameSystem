using Core.Common.State;
using Core.Service.Extension;
using Core.Service.Model;
using Core.Service.Repository.State;

namespace Core.Service.Tests {
	public static class StateRepository {
		public static UserId ValidUserId => new UserId("ValidUserId");
	}

	public static class StateRepository<TState> where TState : IState, new() {
		public static IStateRepository<TState> Create() {
			var settings   = JsonRepositoryDecoratorSettings.Create<TState>();
			var repository = new InMemoryStateRepository<TState>(settings);
			repository.AddForUserId(StateRepository.ValidUserId, new TState());
			return repository;
		}
	}
}
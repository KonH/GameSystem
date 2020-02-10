using Core.Common.State;

namespace Core.Service.Repository.State {
	public sealed class InMemoryStateRepository<TState> :
		InMemoryRepositoryDecorator<TState, string>, IStateRepository<TState> where TState : IState {
		public InMemoryStateRepository(Settings settings) : base(settings) { }
	}
}
using Core.Common.State;

namespace Core.Service.Repository.State {
	public interface IStateRepository<TState> : IRepository<TState> where TState : IState { }
}
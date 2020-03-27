using System.Threading.Tasks;
using Core.Common.State;
using Core.Service.Model;
using Core.Service.Repository.State;

namespace Core.Service.Extension {
	public static class StateRepositoryExtension {
		public static Task Add<TState>(
			this IStateRepository<TState> repository, UserId userId, TState state)
			where TState : IState {
			return repository.Add(userId.Value, state);
		}

		public static Task Update<TState>(
			this IStateRepository<TState> repository, UserId userId, TState state)
			where TState : IState {
			return repository.Update(userId.Value, state);
		}

		public static Task<TState> Get<TState>(
			this IStateRepository<TState> repository, UserId userId)
			where TState : IState {
			return repository.Get(userId.Value);
		}
	}
}
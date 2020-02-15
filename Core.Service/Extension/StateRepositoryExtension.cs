using Core.Common.State;
using Core.Service.Model;
using Core.Service.Repository.State;

namespace Core.Service.Extension {
	public static class StateRepositoryExtension {
		public static void Add<TState>(
			this IStateRepository<TState> repository, UserId userId, TState state)
			where TState : IState {
			repository.Add(userId.Value, state);
		}

		public static void Update<TState>(
			this IStateRepository<TState> repository, UserId userId, TState state)
			where TState : IState {
			repository.Update(userId.Value, state);
		}

		public static TState Get<TState>(
			this IStateRepository<TState> repository, UserId userId)
			where TState : IState {
			return repository.Get(userId.Value);
		}
	}
}
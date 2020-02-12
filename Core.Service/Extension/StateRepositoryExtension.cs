using Core.Common.State;
using Core.Service.Model;
using Core.Service.Repository.State;

namespace Core.Service.Extension {
	public static class StateRepositoryExtension {
		public static void AddForUserId<TState>(
			this IStateRepository<TState> repository, UserId userId, TState state)
			where TState : IState {
			repository.Add(userId.Value, state);
		}

		public static void UpdateForUserId<TState>(
			this IStateRepository<TState> repository, UserId userId, TState state)
			where TState : IState {
			repository.Update(userId.Value, state);
		}

		public static TState GetByUserId<TState>(
			this IStateRepository<TState> repository, UserId userId)
			where TState : IState {
			return repository.Get(userId.Value);
		}
	}
}
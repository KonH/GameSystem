using System;
using Core.Common.State;

namespace Core.Client {
	public sealed class StateFactory<TState> where TState : IState {
		readonly Func<TState> _initializer;

		public StateFactory(Func<TState> initializer) {
			_initializer = initializer;
		}

		public TState Create() {
			return _initializer();
		}
	}
}
using System;
using System.Collections.Generic;
using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Common.CommandDependency {
	public sealed class CommandQueue<TConfig, TState> where TConfig : IConfig where TState : IState {
		public sealed class Dependency {
			public readonly Func<ICommand<TConfig, TState>, bool>                      Condition;
			public readonly Func<ICommand<TConfig, TState>, ICommand<TConfig, TState>> Initializer;

			public Dependency(Func<ICommand<TConfig, TState>, bool> condition,
				Func<ICommand<TConfig, TState>, ICommand<TConfig, TState>> initializer) {
				Condition = condition;
				Initializer = initializer;
			}
		}

		readonly Dictionary<Type, List<Dependency>> _dependencies = new Dictionary<Type, List<Dependency>>();

		public Dictionary<Type, List<Dependency>> Dependencies => _dependencies;

		public CommandQueue<TConfig, TState> AddDependency<TSource, TTarget>(Func<TSource, TTarget> initializer) =>
			AddDependency(c => true, initializer);

		public CommandQueue<TConfig, TState> AddDependency<TSource, TTarget>(Func<TSource, bool> condition, Func<TSource, TTarget> initializer) {
			if ( !_dependencies.TryGetValue(typeof(TSource), out var collection) ) {
				collection = new List<Dependency>();
				_dependencies.Add(typeof(TSource), collection);
			}
			var dependency = new Dependency(
				ic => condition((TSource) ic),
				ic => (ICommand<TConfig, TState>) initializer((TSource) ic));
			collection.Add(dependency);
			return this;
		}
	}
}
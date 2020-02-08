using System;
using System.Collections.Generic;
using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Common.CommandDependency {
	public class CommandQueue<TConfig, TState> where TConfig : IConfig where TState : IState {
		public delegate bool Condition(TConfig config, TState state, ICommand<TConfig, TState> c);

		public delegate ICommand<TConfig, TState> Initializer(
			TConfig config, TState state, ICommand<TConfig, TState> source);

		public sealed class Dependency {
			public readonly Condition   Condition;
			public readonly Initializer Initializer;

			public Dependency(Condition condition, Initializer initializer) {
				Condition = condition;
				Initializer = initializer;
			}
		}

		readonly Dictionary<Type, List<Dependency>> _dependencies = new Dictionary<Type, List<Dependency>>();

		public Dictionary<Type, List<Dependency>> Dependencies => _dependencies;

		public CommandQueue<TConfig, TState> AddDependency<TSource, TTarget>(Func<TSource, TTarget> initializer) =>
			AddDependency(c => true, initializer);

		public CommandQueue<TConfig, TState> AddDependency<TSource, TTarget>(Func<TConfig, TState, TSource, TTarget> initializer) =>
			AddDependency(c => true, initializer);

		public CommandQueue<TConfig, TState> AddDependency<TSource, TTarget>(
			Func<TSource, bool> condition, Func<TSource, TTarget> initializer) =>
			AddDependency<TSource, TTarget>((_, __, c) => condition(c), (_, __, c) => initializer(c));

		public CommandQueue<TConfig, TState> AddDependency<TSource, TTarget>(
			Func<TConfig, TState, TSource, bool> condition, Func<TSource, TTarget> initializer) =>
			AddDependency(condition, (_, __, c) => initializer(c));

		public CommandQueue<TConfig, TState> AddDependency<TSource, TTarget>(
			Func<TSource, bool> condition, Func<TConfig, TState, TSource, TTarget> initializer) =>
			AddDependency((_, __, c) => condition(c), initializer);

		public CommandQueue<TConfig, TState> AddDependency<TSource, TTarget>(
			Func<TConfig, TState, TSource, bool> condition, Func<TConfig, TState, TSource, TTarget> initializer) {
			if ( !_dependencies.TryGetValue(typeof(TSource), out var collection) ) {
				collection = new List<Dependency>();
				_dependencies.Add(typeof(TSource), collection);
			}
			var dependency = new Dependency(
				(c, s, ic) => condition(c, s, (TSource) ic),
				(c, s, ic) => (ICommand<TConfig, TState>) initializer(c, s, (TSource) ic));
			collection.Add(dependency);
			return this;
		}
	}
}
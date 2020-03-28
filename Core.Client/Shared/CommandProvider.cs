using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;
using Core.Common.Utils;

namespace Core.Client.Shared {
	public sealed class CommandProvider<TConfig, TState> where TConfig : IConfig where TState : IState {
		public readonly IReadOnlyList<Type> CommandTypes;

		public CommandProvider(Assembly typeAssembly) {
			CommandTypes = TypeResolver.GetSubclasses<ICommand<TConfig, TState>>(typeAssembly)
				.Where(t => t.GetCustomAttribute<TrustedCommandAttribute>() != null)
				.ToArray();
		}
	}
}
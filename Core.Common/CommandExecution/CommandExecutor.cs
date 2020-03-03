using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;

namespace Core.Common.CommandExecution {
	public sealed class CommandExecutor<TConfig, TState>
		where TState : IState where TConfig : IConfig {
		struct Handler {
			public readonly MethodInfo   BeforeMethod;
			public readonly MethodInfo   AfterMethod;
			public readonly List<object> Instances;

			public Handler(MethodInfo beforeMethod, MethodInfo afterMethod) {
				BeforeMethod = beforeMethod;
				AfterMethod  = afterMethod;
				Instances    = new List<object>();
			}
		}

		readonly object[] _args = new object[2];

		readonly Dictionary<Type, Handler> _handlers = new Dictionary<Type, Handler>();

		public void AddHandler<TCommand>(ICommandHandler<TConfig, TState, TCommand> handler)
			where TCommand : ICommand<TConfig, TState> {
			var commandType = typeof(TCommand);
			if ( !_handlers.TryGetValue(commandType, out var handlerData) ) {
				var handlerType = handler.GetType();
				var before = handlerType.GetMethod(nameof(handler.Before));
				var after  = handlerType.GetMethod(nameof(handler.After));
				handlerData = new Handler(before, after);
				_handlers.Add(commandType, handlerData);
			}
			handlerData.Instances.Add(handler);
		}

		public async Task<CommandResult> Apply<TCommand>(
			TConfig config, TState state, TCommand command, bool foreground = false)
			where TCommand : ICommand<TConfig, TState> {
			if ( config == null ) {
				return CommandResult.BadCommand("Config is invalid");
			}
			if ( state == null ) {
				return CommandResult.BadCommand("State is invalid");
			}
			if ( command == null ) {
				return CommandResult.BadCommand("Command is invalid");
			}
			if ( foreground ) {
				await HandleBefore(state, command);
			}
			var commandResult = command.Apply(config, state);
			if ( commandResult is CommandResult.OkResult ) {
				if ( foreground ) {
					await HandleAfter(state, command);
				}
				state.Version = new StateVersion(state.Version.Value + 1);
			}
			return commandResult;
		}

		async Task HandleBefore<TCommand>(TState state, TCommand command)
			where TCommand : ICommand<TConfig, TState> {
			if ( _handlers.TryGetValue(command.GetType(), out var handler) ) {
				var method = handler.BeforeMethod;
				foreach ( object instance in handler.Instances ) {
					_args[0] = state;
					_args[1] = command;
					await (Task)method.Invoke(instance, _args);
				}
			}
		}

		async Task HandleAfter<TCommand>(TState state, TCommand command)
			where TCommand : ICommand<TConfig, TState> {
			if ( _handlers.TryGetValue(command.GetType(), out var handler) ) {
				var method = handler.AfterMethod;
				foreach ( object instance in handler.Instances ) {
					_args[0] = state;
					_args[1] = command;
					await (Task)method.Invoke(instance, _args);
				}
			}
		}
	}
}
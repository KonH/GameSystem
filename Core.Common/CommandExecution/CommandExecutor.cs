using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;
using Core.Common.Utils;

namespace Core.Common.CommandExecution {
	public sealed class CommandExecutor<TConfig, TState>
		where TState : IState where TConfig : IConfig {
		struct Reaction {
			public readonly MethodInfo   BeforeMethod;
			public readonly MethodInfo   AfterMethod;
			public readonly List<object> Instances;

			public Reaction(MethodInfo beforeMethod, MethodInfo afterMethod) {
				BeforeMethod = beforeMethod;
				AfterMethod  = afterMethod;
				Instances    = new List<object>();
			}
		}

		readonly object[] _args = new object[4];

		readonly Dictionary<Type, Reaction> _reactions = new Dictionary<Type, Reaction>();

		readonly ILogger<CommandExecutor<TConfig, TState>> _logger;

		public CommandExecutor(ILoggerFactory loggerFactory) {
			_logger = loggerFactory.Create<CommandExecutor<TConfig, TState>>();
		}

		public void ClearReactions() {
			_reactions.Clear();
		}

		public void AddReaction<TCommand>(ICommandReaction<TConfig, TState, TCommand> reaction)
			where TCommand : ICommand<TConfig, TState> {
			var commandType = typeof(TCommand);
			if ( !_reactions.TryGetValue(commandType, out var reactionData) ) {
				var reactionType = reaction.GetType();
				var before = reactionType.GetMethod(nameof(reaction.Before));
				var after  = reactionType.GetMethod(nameof(reaction.After));
				reactionData = new Reaction(before, after);
				_reactions.Add(commandType, reactionData);
			}
			reactionData.Instances.Add(reaction);
		}

		public async Task<CommandResult> Apply<TCommand>(
			TConfig config, TState state, TCommand command, bool foreground, CancellationToken cancellationToken)
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
				await HandleBefore(config, state, command, cancellationToken);
			}
			cancellationToken.ThrowIfCancellationRequested();
			var commandResult = command.Apply(config, state);
			if ( commandResult is CommandResult.OkResult ) {
				if ( foreground ) {
					await HandleAfter(config, state, command, cancellationToken);
				}
				cancellationToken.ThrowIfCancellationRequested();
				state.Version = new StateVersion(state.Version.Value + 1);
			}
			return commandResult;
		}

		public Task<CommandResult> Apply<TCommand>(
			TConfig config, TState state, TCommand command, bool foreground = false)
			where TCommand : ICommand<TConfig, TState> {
			return Apply(config, state, command, foreground, CancellationToken.None);
		}

		async Task HandleBefore<TCommand>(TConfig config, TState state, TCommand command, CancellationToken cancellationToken)
			where TCommand : ICommand<TConfig, TState> {
			if ( _reactions.TryGetValue(command.GetType(), out var reaction) ) {
				var method = reaction.BeforeMethod;
				foreach ( object instance in reaction.Instances ) {
					cancellationToken.ThrowIfCancellationRequested();
					try {
						_args[0] = config;
						_args[1] = state;
						_args[2] = command;
						_args[3] = cancellationToken;
						await (Task)method.Invoke(instance, _args);
					} catch ( Exception e ) {
						_logger.LogError(e.ToString());
					}
				}
			}
		}

		async Task HandleAfter<TCommand>(TConfig config, TState state, TCommand command, CancellationToken cancellationToken)
			where TCommand : ICommand<TConfig, TState> {
			if ( _reactions.TryGetValue(command.GetType(), out var reaction) ) {
				var method = reaction.AfterMethod;
				foreach ( object instance in reaction.Instances ) {
					cancellationToken.ThrowIfCancellationRequested();
					try {
						_args[0] = config;
						_args[1] = state;
						_args[2] = command;
						_args[3] = cancellationToken;
						await (Task)method.Invoke(instance, _args);
					} catch ( Exception e ) {
						_logger.LogError(e.ToString());
					}
				}
			}
		}
	}
}
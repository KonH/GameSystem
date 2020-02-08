using System.Collections.Generic;
using Core.Common.Command;
using Core.Common.CommandDependency;
using Core.Common.Config;
using Core.Common.State;
using Core.Common.Utils;

namespace Core.Common.CommandExecution {
	public sealed class BatchCommandExecutor<TConfig, TState>
		where TState : IState where TConfig : IConfig {
		readonly CommandExecutor<TConfig, TState> _executor = new CommandExecutor<TConfig, TState>();

		readonly ILogger<BatchCommandExecutor<TConfig, TState>> _logger;
		readonly CommandHandler<TConfig, TState>                _handler;

		public BatchCommandExecutor(
			ILogger<BatchCommandExecutor<TConfig, TState>> logger,
			CommandQueue<TConfig, TState> queue) {
			_logger = logger;
			_handler = new CommandHandler<TConfig, TState>(queue);
		}

		public BatchCommandResult<TConfig, TState> Apply<TCommand>(TConfig config, TState state, TCommand command)
			where TCommand : ICommand<TConfig, TState> {
			_logger.LogTrace($"Applying command: '{command}'");
			var commandResult = _executor.Apply(config, state, command);
			if ( commandResult is CommandResult.BadCommandResult badCommand ) {
				return BatchCommandResult<TConfig, TState>.BadCommand(badCommand.Description);
			}
			var dependencies = _handler.GetDependentCommands(command);
			var accum = new List<ICommand<TConfig, TState>>();
			foreach ( var dependency in dependencies ) {
				var dependencyResult = Apply(config, state, dependency);
				if ( dependencyResult is BatchCommandResult<TConfig, TState>.BadCommandResult ) {
					return dependencyResult;
				}
				_logger.LogTrace($"Add dependency: '{dependency}'");
				accum.Add(dependency);
				var okResult = (BatchCommandResult<TConfig, TState>.OkResult) dependencyResult;
				_logger.LogTrace(
					$"Add dependencies from command: '{dependency}' = {string.Join(',', okResult.NextCommands)}");
				accum.AddRange(okResult.NextCommands);
			}
			return BatchCommandResult<TConfig, TState>.Ok(accum);
		}
	}
}
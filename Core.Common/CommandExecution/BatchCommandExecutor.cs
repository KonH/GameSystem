using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Common.Command;
using Core.Common.CommandDependency;
using Core.Common.Config;
using Core.Common.State;
using Core.Common.Utils;

namespace Core.Common.CommandExecution {
	public sealed class BatchCommandExecutor<TConfig, TState>
		where TState : IState where TConfig : IConfig {
		readonly ILogger<BatchCommandExecutor<TConfig, TState>> _logger;
		readonly CommandExecutor<TConfig, TState>               _executor;
		readonly CommandDependencyHandler<TConfig, TState>      _dependencyHandler;

		public BatchCommandExecutor(
			ILoggerFactory loggerFactory, CommandExecutor<TConfig, TState> commandExecutor,
			CommandQueue<TConfig, TState> queue) {
			_logger            = loggerFactory.Create<BatchCommandExecutor<TConfig, TState>>();
			_executor          = commandExecutor;
			_dependencyHandler = new CommandDependencyHandler<TConfig, TState>(queue);
		}

		public async Task<BatchCommandResult> Apply<TCommand>(
			TConfig config, TState state, TCommand command, bool foreground = false)
			where TCommand : ICommand<TConfig, TState> {
			_logger.LogTrace($"Applying command: '{command}'");
			var commandResult = await _executor.Apply(config, state, command, foreground);
			if ( commandResult is CommandResult.BadCommandResult badCommand ) {
				_logger.LogWarning($"Command '{command}' failed: '{badCommand.Description}'");
				return new BatchCommandResult.BadCommand(badCommand.Description);
			}
			var dependencies = _dependencyHandler.GetDependentCommands(config, state, command);
			var accum        = new List<ICommand<TConfig, TState>>();
			foreach ( var dependency in dependencies ) {
				var dependencyResult = await Apply(config, state, dependency, foreground);
				if ( dependencyResult is BatchCommandResult.BadCommand ) {
					return dependencyResult;
				}
				_logger.LogTrace($"Add dependency: '{dependency}'");
				accum.Add(dependency);
				var okResult = (BatchCommandResult.Ok<TConfig, TState>) dependencyResult;
				_logger.LogTrace(
					$"Add dependencies from command: '{dependency}' = {string.Join(",", okResult.NextCommands)}");
				accum.AddRange(okResult.NextCommands);
			}
			return new BatchCommandResult.Ok<TConfig, TState>(accum);
		}
	}
}
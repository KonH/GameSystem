using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Client.Abstractions;
using Core.Client.Shared;
using Core.Common.Command;
using Core.Common.CommandExecution;
using Core.Common.Config;
using Core.Common.State;
using Core.Common.Threading;
using Core.Common.Utils;
using Core.Service.Model;
using Core.Service.UseCase.GetConfig;
using Core.Service.UseCase.GetState;
using Core.Service.UseCase.SendCommand;
using Core.Service.UseCase.WaitCommand;

namespace Core.Client.Web {
	public sealed class WebServiceWaitClient<TConfig, TState> : IClient<TConfig, TState>
		where TConfig : IConfig where TState : class, IState {
		readonly UserId _userId;

		readonly ILogger<WebServiceWaitClient<TConfig, TState>> _logger;
		readonly CommandExecutor<TConfig, TState>               _singleExecutor;
		readonly WebClientHandler                               _webClientHandler;
		readonly ITaskRunner                                    _taskRunner;

		TaskCompletionSource<bool> _mainRequestCompletionSource = null;
		TaskCompletionSource<bool> _updateStateCompletionSource = null;

		public TState  State  { get; private set; }
		public TConfig Config { get; private set; }

		public event Action StateUpdated = () => {};

		public WebServiceWaitClient(
			ILoggerFactory loggerFactory, CommandExecutor<TConfig, TState> commandExecutor,
			WebClientHandler webClientHandler, UserIdSource userIdSource, ITaskRunner taskRunner) {
			_userId           = userIdSource.GetOrCreateUserId();
			_logger           = loggerFactory.Create<WebServiceWaitClient<TConfig, TState>>();
			_singleExecutor   = commandExecutor;
			_webClientHandler = webClientHandler;
			_taskRunner       = taskRunner;
		}

		public async Task<InitializationResult> Initialize(CancellationToken cancellationToken) {
			try {
				await UpdateConfig(cancellationToken);
				await UpdateState(cancellationToken);
				_taskRunner.Run(WaitCommand, cancellationToken);
			} catch ( Exception e ) {
				return new InitializationResult.Error(e.ToString());
			}
			return new InitializationResult.Ok();
		}

		public async Task<CommandApplyResult> Apply(ICommand<TConfig, TState> command, CancellationToken cancellationToken) {
			await WaitForStateUpdate();
			var request = new SendCommandRequest<TConfig, TState>(_userId, State.Version, Config.Version, command);
			var response = await PerformMainRequest(() => _webClientHandler.SendCommand(request));
			switch ( response ) {
				case SendCommandResponse.Applied _: {
					return new CommandApplyResult.Ok();
				}

				case SendCommandResponse.Rejected rejected: {
					return new CommandApplyResult.Error($"Command rejected: '{rejected.Description}'");
				}

				case SendCommandResponse.BadRequest badRequest: {
					return new CommandApplyResult.Error($"Command is invalid: '{badRequest.Description}'");
				}

				case object value: {
					return new CommandApplyResult.Error($"Result is '{value.GetType()}', force update state");
				}

				default: {
					return new CommandApplyResult.Error("Failed to apply command");
				}
			}
		}

		async Task WaitCommand(CancellationToken cancellationToken) {
			while ( true ) {
				cancellationToken.ThrowIfCancellationRequested();
				_logger.LogTrace("Awaiting for new command");
				await WaitForStateUpdate();
				await WaitForMainRequest();
				var request  = new WaitCommandRequest(_userId, State.Version, Config.Version);
				var response = await _webClientHandler.WaitCommand(request);
				cancellationToken.ThrowIfCancellationRequested();
				switch ( response ) {
					case WaitCommandResponse.Updated<TConfig, TState> updated: {
						_logger.LogTrace($"New commands found: {updated.NextCommands.Length}");
						await UpdateState(updated.NewVersion, async () => {
							foreach ( var cmd in updated.NextCommands ) {
								await _singleExecutor.Apply(Config, State, cmd, true, cancellationToken);
							}
						});
						foreach ( var error in updated.Errors ) {
							_logger.LogError(error.ToString());
						}
						StateUpdated.Invoke();
						break;
					}

					case WaitCommandResponse.NotFound _: {
						_logger.LogTrace("No new commands found");
						break;
					}

					case WaitCommandResponse.Outdated _: {
						_logger.LogTrace($"Command outdated");
						break;
					}

					case WaitCommandResponse.BadRequest badRequest: {
						_logger.LogTrace($"Command failed: '{badRequest.Description}'");
						break;
					}

					case object value: {
						_logger.LogError($"New command result is '{value.GetType()}'");
						break;
					}

					default: {
						_logger.LogError("Failed to get new command");
						break;
					}
				}
			}
			// ReSharper disable once FunctionNeverReturns
		}

		async Task UpdateConfig(CancellationToken cancellationToken) {
			_logger.LogTrace($"Update config for '{_userId}'");
			var request  = new GetConfigRequest(_userId);
			var response = await _webClientHandler.GetConfig(request);
			cancellationToken.ThrowIfCancellationRequested();
			switch ( response ) {
				case GetConfigResponse.Found<TConfig> found: {
					Config = found.Config;
					_logger.LogTrace("Config found");
					break;
				}

				case object value: {
					_logger.LogError($"Result is '{value.GetType()}'");
					throw new InvalidOperationException();
				}
			}
		}

		async Task UpdateState(CancellationToken cancellationToken) {
			_logger.LogTrace($"Update state for '{_userId}'");
			var request  = new GetStateRequest(_userId);
			var response = await _webClientHandler.GetState(request);
			cancellationToken.ThrowIfCancellationRequested();
			switch ( response ) {
				case GetStateResponse.Found<TState> found: {
					State = found.State;
					_logger.LogTrace("State found");
					break;
				}

				case GetStateResponse.BadRequest badRequest: {
					_logger.LogError($"Failed to get state: '{badRequest.Description}'");
					throw new InvalidOperationException();
				}

				case object value: {
					_logger.LogError($"Result is '{value.GetType()}'");
					throw new InvalidOperationException();
				}
			}
		}

		async Task WaitForMainRequest() {
			if ( _mainRequestCompletionSource != null ) {
				var task = _mainRequestCompletionSource.Task;
				_logger.LogTrace("Main request in progress, waiting for complete");
				await task;
				_logger.LogTrace("Main request finished");
			}
		}

		async Task<T> PerformMainRequest<T>(Func<Task<T>> callback) {
			try {
				await WaitForMainRequest();
				_mainRequestCompletionSource = new TaskCompletionSource<bool>();
				return await callback();
			} finally {
				_mainRequestCompletionSource?.TrySetResult(true);
				_mainRequestCompletionSource = null;
			}
		}

		async Task WaitForStateUpdate() {
			if ( _updateStateCompletionSource != null ) {
				var task = _updateStateCompletionSource.Task;
				_logger.LogTrace("Update state in progress, waiting for complete");
				await task;
				_logger.LogTrace("Update state finished");
			}
		}

		async Task UpdateState(StateVersion updatedVersion, Func<Task> callback) {
			try {
				await WaitForStateUpdate();
				_updateStateCompletionSource = new TaskCompletionSource<bool>();
				await callback();
				State.Version = new StateVersion(Math.Max(State.Version.Value, updatedVersion.Value));
			} finally {
				_updateStateCompletionSource?.TrySetResult(true);
				_updateStateCompletionSource = null;
			}
		}
	}
}
using System;
using System.Reflection;
using System.Threading.Tasks;
using Core.Common.Command;
using Core.Common.Config;
using Core.Common.State;
using Core.Service.Extension;
using Core.Service.Queue;
using Core.Service.Repository.Config;

namespace Core.Service.UseCase.SendCommand {
	public sealed class
		SendCommandUseCase<TConfig, TState> :
			IUseCase<SendCommandRequest<TConfig, TState>, SendCommandResponse>
		where TConfig : IConfig where TState : class, IState, new() {
		readonly IConfigRepository<TConfig>     _configRepository;
		readonly CommonWatcher<TConfig, TState> _watcher;

		public SendCommandUseCase(IConfigRepository<TConfig> configRepository, CommonWatcher<TConfig, TState> watcher) {
			_configRepository = configRepository;
			_watcher          = watcher;
		}

		public async Task<SendCommandResponse> Handle(SendCommandRequest<TConfig, TState> request) {
			var validateError = await Validate(request);
			if ( validateError != null ) {
				return validateError;
			}
			var commandType = request.Command.GetType();
			if ( !IsTrustedCommand(commandType) ) {
				return Rejected($"Command of type '{commandType.FullName}' isn't trusted");
			}
			_watcher.AddCommand(request.UserId, request.Command);
			return Applied();
		}

		async Task<SendCommandResponse> Validate(SendCommandRequest<TConfig, TState> request) {
			if ( request == null ) {
				return BadRequest("null request");
			}
			if ( request.Command == null ) {
				return BadRequest("null command");
			}
			var config = await _configRepository.Get(request.ConfigVersion);
			if ( config == null ) {
				return BadRequest($"Config '{request.ConfigVersion}' isn't found");
			}
			return null;
		}

		static SendCommandResponse Applied() {
			return new SendCommandResponse.Applied();
		}

		static SendCommandResponse Rejected(string description) {
			return new SendCommandResponse.Rejected(description);
		}

		static SendCommandResponse BadRequest(string description) {
			return new SendCommandResponse.BadRequest(description);
		}

		static bool IsTrustedCommand(Type type) {
			return type.GetCustomAttribute<TrustedCommandAttribute>() != null;
		}
	}
}
using System.Threading;
using System.Threading.Tasks;
using Idler.Common.Command;
using Idler.Common.Config;
using Idler.Common.State;
using Idler.UnityClient.View;
using Core.Client.UnityClient.CommandExecution;

namespace Idler.UnityClient.Reaction {
	public sealed class AddSharedResourceCommandReaction : UnityCommandReaction<GameConfig, GameState, AddSharedResourceCommand> {
		readonly SharedResourceView           _resourceView;
		readonly SendSharedResourceButtonView _sendSharedResourceButtonView;

		public AddSharedResourceCommandReaction(SharedResourceView resourceView, SendSharedResourceButtonView sendSharedResourceButtonView) {
			_resourceView                 = resourceView;
			_sendSharedResourceButtonView = sendSharedResourceButtonView;
		}

		public override Task BeforeOnMainThread(GameConfig config, GameState state, AddSharedResourceCommand command, CancellationToken cancellationToken) {
			return _resourceView.AppearValue(1, cancellationToken);
		}

		public override async Task AfterOnMainThread(GameConfig config, GameState state, AddSharedResourceCommand command, CancellationToken cancellationToken) {
			await _resourceView.AnimateValue(state, cancellationToken);
			_sendSharedResourceButtonView.UpdateState(config, state);
		}
	}

	public sealed class SendSharedResourceCommandReaction : UnityCommandReaction<GameConfig, GameState, SendSharedResourceCommand> {
		readonly SharedResourceRemoteView _resourceView;

		public SendSharedResourceCommandReaction(SharedResourceRemoteView resourceView) {
			_resourceView = resourceView;
		}

		public override Task AfterOnMainThread(GameConfig config, GameState state, SendSharedResourceCommand command, CancellationToken cancellationToken) {
			return _resourceView.UpdateValue();
		}
	}
}
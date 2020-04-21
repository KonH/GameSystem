using System.Threading;
using System.Threading.Tasks;
using Core.Client.UnityClient.CommandExecution;
using Idler.Common.Command;
using Idler.Common.Config;
using Idler.Common.State;
using Idler.UnityClient.View;

namespace Idler.UnityClient.Reaction {
	public sealed class RemoveSharedResourceCommandReaction : UnityCommandReaction<GameConfig, GameState, RemoveSharedResourceCommand> {
		readonly SharedResourceView           _sharedResourceView;
		readonly SendSharedResourceButtonView _sendSharedResourceView;

		public RemoveSharedResourceCommandReaction(SharedResourceView sharedResourceView, SendSharedResourceButtonView sendSharedResourceView) {
			_sharedResourceView     = sharedResourceView;
			_sendSharedResourceView = sendSharedResourceView;
		}

		public override Task BeforeOnMainThread(GameConfig config, GameState state, RemoveSharedResourceCommand command, CancellationToken cancellationToken) {
			return _sharedResourceView.AppearValue(-1, cancellationToken);
		}

		public override async Task AfterOnMainThread(GameConfig config, GameState state, RemoveSharedResourceCommand command, CancellationToken cancellationToken) {
			await _sharedResourceView.AnimateValue(state, cancellationToken);
			cancellationToken.ThrowIfCancellationRequested();
			_sendSharedResourceView.UpdateState(config, state);
		}
	}
}
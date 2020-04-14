using System.Threading;
using System.Threading.Tasks;
using Core.Client.UnityClient.CommandExecution;
using Idler.Common.Command;
using Idler.Common.Config;
using Idler.Common.State;
using Idler.UnityClient.View;

namespace Idler.UnityClient.Reaction {
	public sealed class RemoveResourceCommandReaction : UnityCommandReaction<GameConfig, GameState, RemoveResourceCommand> {
		readonly ResourceView                _resourceView;
		readonly AddSharedResourceButtonView _addSharedResourceView;

		public RemoveResourceCommandReaction(ResourceView resourceView, AddSharedResourceButtonView addSharedResourceView) {
			_resourceView          = resourceView;
			_addSharedResourceView = addSharedResourceView;
		}

		public override Task BeforeOnMainThread(GameConfig config, GameState state, RemoveResourceCommand command, CancellationToken cancellationToken) {
			return _resourceView.AppearValue(-command.Amount, cancellationToken);
		}

		public override async Task AfterOnMainThread(GameConfig config, GameState state, RemoveResourceCommand command, CancellationToken cancellationToken) {
			await _resourceView.AnimateValue(state, cancellationToken);
			cancellationToken.ThrowIfCancellationRequested();
			_addSharedResourceView.UpdateState(config, state);
		}
	}
}
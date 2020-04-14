using System.Threading;
using System.Threading.Tasks;
using Clicker.Common.Command;
using Clicker.Common.Config;
using Clicker.Common.State;
using Clicker.UnityClient.View;
using Core.Client.UnityClient.CommandExecution;

namespace Clicker.UnityClient.Reaction {
	public sealed class RemoveResourceCommandReaction :
		UnityCommandReaction<GameConfig, GameState, RemoveResourceCommand> {
		readonly ResourceView      _resourceView;
		readonly UpgradeButtonView _upgradeView;

		public RemoveResourceCommandReaction(ResourceView resourceView, UpgradeButtonView upgradeView) {
			_resourceView = resourceView;
			_upgradeView  = upgradeView;
		}

		public override Task BeforeOnMainThread(GameConfig config, GameState state, RemoveResourceCommand command, CancellationToken cancellationToken) {
			return _resourceView.AppearValue(-command.Amount, cancellationToken);
		}

		public override async Task AfterOnMainThread(
			GameConfig config, GameState state, RemoveResourceCommand command, CancellationToken cancellationToken) {
			await _resourceView.AnimateValue(state, cancellationToken);
			cancellationToken.ThrowIfCancellationRequested();
			_upgradeView.UpdateState(config, state);
		}
	}
}
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

		public override Task BeforeOnMainThread(GameConfig config, GameState state, RemoveResourceCommand command) {
			return _resourceView.AppearValue(-command.Amount);
		}

		public override async Task AfterOnMainThread(
			GameConfig config, GameState state, RemoveResourceCommand command) {
			await _resourceView.AnimateValue(state);
			_upgradeView.UpdateState(config, state);
		}
	}
}
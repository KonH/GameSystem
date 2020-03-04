using System.Threading.Tasks;
using Clicker.Common.Command;
using Clicker.Common.Config;
using Clicker.Common.State;
using Clicker.UnityClient.View;
using Core.Client.UnityClient.CommandExecution;

namespace Clicker.UnityClient.Handler {
	public sealed class UpgradeCommandHandler : UnityCommandHandler<GameConfig, GameState, UpgradeCommand> {
		readonly UpgradeLevelView _view;

		public UpgradeCommandHandler(UpgradeLevelView view) {
			_view = view;
		}

		public override Task AfterOnMainThread(GameConfig config, GameState state, UpgradeCommand command) {
			return _view.AnimateValue(config, state);
		}
	}
}
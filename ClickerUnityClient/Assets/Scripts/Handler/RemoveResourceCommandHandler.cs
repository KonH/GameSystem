using System.Threading.Tasks;
using Clicker.Common.Command;
using Clicker.Common.Config;
using Clicker.Common.State;
using Clicker.UnityClient.View;
using Core.Client.UnityClient.CommandExecution;

namespace Clicker.UnityClient.Handler {
	public sealed class RemoveResourceCommandHandler : UnityCommandHandler<GameConfig, GameState, RemoveResourceCommand> {
		readonly ResourceView _view;

		public RemoveResourceCommandHandler(ResourceView view) {
			_view = view;
		}

		public override Task AfterOnMainThread(GameConfig config, GameState state, RemoveResourceCommand command) {
			return _view.AnimateValue(state);
		}
	}
}
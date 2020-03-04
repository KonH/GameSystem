using System.Threading.Tasks;
using Clicker.Common.Command;
using Clicker.Common.Config;
using Clicker.Common.State;
using Clicker.UnityClient.View;
using Core.Client.UnityClient.CommandExecution;

namespace Clicker.UnityClient.Handler {
	public sealed class AddResourceCommandHandler : UnityCommandHandler<GameConfig, GameState, AddResourceCommand> {
		readonly ResourceView _view;

		public AddResourceCommandHandler(ResourceView view) {
			_view = view;
		}

		public override Task BeforeOnMainThread(GameConfig config, GameState state, AddResourceCommand command) {
			return _view.AppearValue(command.Amount);
		}

		public override Task AfterOnMainThread(GameConfig config, GameState state, AddResourceCommand command) {
			return _view.AnimateValue(state);
		}
	}
}
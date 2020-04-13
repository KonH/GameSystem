using System.Threading.Tasks;
using Idler.Common.Command;
using Idler.Common.Config;
using Idler.Common.State;
using Idler.UnityClient.View;
using Core.Client.UnityClient.CommandExecution;

namespace Idler.UnityClient.Reaction {
	public sealed class AddResourceCommandReaction : UnityCommandReaction<GameConfig, GameState, AddResourceCommand> {
		readonly ResourceView                _resourceView;
		readonly AddSharedResourceButtonView _addSharedResourceView;

		public AddResourceCommandReaction(ResourceView resourceView, AddSharedResourceButtonView addSharedResourceView) {
			_resourceView          = resourceView;
			_addSharedResourceView = addSharedResourceView;
		}

		public override Task BeforeOnMainThread(GameConfig config, GameState state, AddResourceCommand command) {
			return _resourceView.AppearValue(command.Amount);
		}

		public override async Task AfterOnMainThread(GameConfig config, GameState state, AddResourceCommand command) {
			await _resourceView.AnimateValue(state);
			_addSharedResourceView.UpdateState(config, state);
		}
	}
}
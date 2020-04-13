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

		public override Task BeforeOnMainThread(GameConfig config, GameState state, RemoveResourceCommand command) {
			return _resourceView.AppearValue(-command.Amount);
		}

		public override async Task AfterOnMainThread(GameConfig config, GameState state, RemoveResourceCommand command) {
			await _resourceView.AnimateValue(state);
			_addSharedResourceView.UpdateState(config, state);
		}
	}
}
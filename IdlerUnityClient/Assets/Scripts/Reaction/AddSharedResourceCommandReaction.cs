using System.Threading.Tasks;
using Idler.Common.Command;
using Idler.Common.Config;
using Idler.Common.State;
using Idler.UnityClient.View;
using Core.Client.UnityClient.CommandExecution;

namespace Idler.UnityClient.Reaction {
	public sealed class AddSharedResourceCommandReaction : UnityCommandReaction<GameConfig, GameState, AddSharedResourceCommand> {
		readonly SharedResourceView _resourceView;

		public AddSharedResourceCommandReaction(SharedResourceView resourceView) {
			_resourceView = resourceView;
		}

		public override Task BeforeOnMainThread(GameConfig config, GameState state, AddSharedResourceCommand command) {
			return _resourceView.AppearValue(1);
		}

		public override async Task AfterOnMainThread(GameConfig config, GameState state, AddSharedResourceCommand command) {
			await _resourceView.AnimateValue(state);
		}
	}
}
using System.Threading.Tasks;
using Clicker.Common.Command;
using Clicker.Common.Config;
using Clicker.Common.State;
using Core.Client.UnityClient.CommandExecution;
using Core.Client.UnityClient.Extension;
using TMPro;
using UnityEngine;

namespace Clicker.UnityClient.Handlers {
	public sealed class AddResourceCommandHandler : UnityCommandHandler<GameConfig, GameState, AddResourceCommand> {
		readonly Animation _appearAnim;
		readonly TMP_Text  _appearText;
		readonly TMP_Text  _countText;
		readonly Animation _countAnim;

		public AddResourceCommandHandler(TMP_Text countText, TMP_Text appearText) {
			_countText  = countText;
			_countAnim  = _countText.GetComponent<Animation>();
			_appearText = appearText;
			_appearAnim = _appearText.GetComponent<Animation>();
		}

		public override async Task BeforeOnMainThread(GameState state, AddResourceCommand command) {
			_appearText.text = $"+{command.Amount}";
			if ( _appearAnim ) {
				await _appearAnim.Wait();
			}
		}

		public override async Task AfterOnMainThread(GameState state, AddResourceCommand command) {
			_countText.text = state.Resource.Resources.ToString();
			if ( _countAnim ) {
				await _countAnim.Wait();
			}
		}
	}
}
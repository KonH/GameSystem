using System.Threading.Tasks;
using Clicker.Common.Command;
using Clicker.Common.Config;
using Clicker.Common.State;
using Core.Client.UnityClient.CommandExecution;
using Core.Client.UnityClient.Extension;
using TMPro;
using UnityEngine;

namespace Clicker.UnityClient.Handler {
	public sealed class UpgradeCommandHandler : UnityCommandHandler<GameConfig, GameState, UpgradeCommand> {
		readonly Animation _appearAnim;
		readonly TMP_Text  _countText;
		readonly Animation _countAnim;

		public UpgradeCommandHandler(TMP_Text countText, TMP_Text appearText) {
			_countText  = countText;
			_countAnim  = _countText.GetComponent<Animation>();
			_appearAnim = appearText.GetComponent<Animation>();
		}

		public override async Task BeforeOnMainThread(GameState state, UpgradeCommand command) {
			if ( _appearAnim ) {
				await _appearAnim.Wait();
			}
		}

		public override async Task AfterOnMainThread(GameState state, UpgradeCommand command) {
			_countText.text = state.Upgrade.Level.ToString();
			if ( _countAnim ) {
				await _countAnim.Wait();
			}
		}
	}
}
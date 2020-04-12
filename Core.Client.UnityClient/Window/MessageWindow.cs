using System.Threading.Tasks;
using Core.Client.UnityClient.Extension;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Client.UnityClient.Window {
	public sealed class MessageWindow : MonoBehaviour {
		[SerializeField]
		Animation _animation = null;

		[SerializeField]
		AnimationClip _showAnimation = null;

		[SerializeField]
		AnimationClip _hideAnimation = null;

		[SerializeField]
		TMP_Text _headerText = null;

		[SerializeField]
		TMP_Text _bodyText = null;

		[SerializeField]
		TMP_Text _buttonText = null;

		[SerializeField]
		Button _button = null;

		TaskCompletionSource<bool> _completionSource;

		public async Task Show(string headerText, string bodyText, string buttonText) {
			_button.onClick.RemoveAllListeners();
			_button.onClick.AddListener(OnClick);
			_headerText.text  = headerText;
			_bodyText.text    = bodyText;
			_buttonText.text  = buttonText;
			_completionSource = new TaskCompletionSource<bool>();
			if ( _animation && _showAnimation ) {
				await _animation.Wait(_showAnimation);
			}
			await _completionSource.Task;
			if ( _animation && _hideAnimation ) {
				await _animation.Wait(_hideAnimation);
			}
		}

		void OnClick() {
			_completionSource.SetResult(true);
		}
	}
}
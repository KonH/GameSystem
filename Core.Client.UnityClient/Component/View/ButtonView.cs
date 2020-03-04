using UnityEngine;
using UnityEngine.UI;

namespace Core.Client.UnityClient.Component.View {
	[RequireComponent(typeof(Button))]
	public class ButtonView : MonoBehaviour {
		Button _button;

		public void Init(bool isActive) {
			Init();
			UpdateState(isActive);
		}

		protected virtual void Init() {
			_button = GetComponent<Button>();
		}

		public void UpdateState(bool isActive) {
			_button.enabled = isActive;
		}
	}
}
using TMPro;
using UnityEngine;

namespace Core.Client.UnityClient.Component.View {
	[RequireComponent(typeof(TMP_Text))]
	public class TextView : MonoBehaviour {
		[SerializeField] string _format = null;

		TMP_Text _text;

		public virtual void Init(string value) {
			Init();
			UpdateValue(value);
		}

		protected virtual void Init() {
			_text = GetComponent<TMP_Text>();
		}

		public virtual void Init(int value) => Init(value.ToString());

		public void UpdateValue(string value) {
			var text = string.IsNullOrEmpty(_format) ? value : string.Format(_format, value);
			_text.text = text;
		}

		public void UpdateValue(int value) => UpdateValue(value.ToString());
	}
}
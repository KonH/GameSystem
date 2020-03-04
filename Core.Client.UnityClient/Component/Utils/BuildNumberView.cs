using TMPro;
using UnityEngine;

namespace Core.Client.UnityClient.Component.Utils {
	[RequireComponent(typeof(TMP_Text))]
	public sealed class BuildNumberView : MonoBehaviour {
		void Start() {
			var text = GetComponent<TMP_Text>();
			text.text = Application.version;
		}
	}
}
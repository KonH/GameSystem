using UnityEngine;
using UnityEngine.UI;

namespace Core.Client.UnityClient.Components.UI {
	[RequireComponent(typeof(Text))]
	public sealed class BuildNumberView : MonoBehaviour {
		void Start() {
			var text = GetComponent<Text>();
			text.text = Application.version;
		}
	}
}
using TMPro;
using UnityEngine;

namespace Core.Client.UnityClient.Components.TMP {
	[RequireComponent(typeof(TMP_Text))]
	public sealed class TmpBuildNumberView : MonoBehaviour {
		void Start() {
			var text = GetComponent<TMP_Text>();
			text.text = Application.version;
		}
	}
}
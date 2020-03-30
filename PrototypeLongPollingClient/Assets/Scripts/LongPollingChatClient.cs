using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace PrototypeLongPollingClient {
	public sealed class LongPollingChatClient : MonoBehaviour {
		[SerializeField]
		InputField MessageInput = null;

		[SerializeField]
		Button SendButton = null;

		[SerializeField]
		Text LogText = null;

		void Awake() {
			SendButton.onClick.AddListener(SendMessage);
		}

		void Start() {
			StartCoroutine(WaitForMessage());
		}

		IEnumerator WaitForMessage() {
			while ( true ) {
				var webRequest = UnityWebRequest.Get("http://localhost:8082/chat");
				webRequest.timeout = 70;
				yield return webRequest.SendWebRequest();
				Debug.Log("WaitForMessage responseCode: " + webRequest.responseCode.ToString());
				if ( webRequest.responseCode == 200 ) {
					Debug.Log($"WaitForMessage text '{webRequest.downloadHandler.text}'");
					var text = webRequest.downloadHandler.text;
					LogText.text += $"\n{text}";
				}
			}
		}

		void SendMessage() {
			StartCoroutine(SendMessageCoro());
		}

		IEnumerator SendMessageCoro() {
			var webRequest = UnityWebRequest.Post($"http://localhost:8082/chat?message={UnityWebRequest.EscapeURL(MessageInput.text)}", new Dictionary<string, string>());
			MessageInput.text = string.Empty;
			yield return webRequest.SendWebRequest();
			Debug.Log("SendMessage responseCode: " + webRequest.responseCode.ToString());
		}
	}
}
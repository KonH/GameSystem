using UnityEngine;

namespace Core.Client.UnityClient.Threading {
	sealed class UnityThreadRunner : MonoBehaviour {
		static UnityThreadRunner _instance;

		public static UnityThreadRunner Instance {
			get {
				if ( _instance == null ) {
					_instance = new GameObject(nameof(UnityThreadRunner))
						.AddComponent<UnityThreadRunner>();
				}

				return _instance;
			}
		}

		void Awake() {
			var go = gameObject;
			go.hideFlags = HideFlags.HideAndDontSave;
			DontDestroyOnLoad(go);
		}
	}
}
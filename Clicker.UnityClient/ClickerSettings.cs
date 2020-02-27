using Core.Client.UnityClient.Settings;
using UnityEngine;

namespace Clicker.UnityClient {
	[CreateAssetMenu]
	public class ClickerSettings : ScriptableObject, ISettings {
		[SerializeField] ClientMode _mode = default;

		[Header("Standalone")] [SerializeField]
		string _configPath = null;

		public ClientMode Mode       => _mode;
		public string     ConfigPath => _configPath;
	}
}
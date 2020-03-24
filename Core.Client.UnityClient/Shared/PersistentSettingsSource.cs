using System.IO;
using Core.Client.Models;
using Core.Client.Shared;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.Client.UnityClient.Shared {
	public sealed class PersistentSettingsSource : ISettingsSource {
		readonly string _relativeFilePath;

		public PersistentSettingsSource(string relativeFilePath = "settings.json") {
			_relativeFilePath = relativeFilePath;
		}

		public CommonSettings GetLocalSettings() {
			var fullPath = Path.Combine(Application.persistentDataPath, _relativeFilePath);
			if ( !File.Exists(fullPath) ) {
				return new CommonSettings();
			}
			var content = File.ReadAllText(fullPath);
			return JsonConvert.DeserializeObject<CommonSettings>(content);
		}

		public void UpdateSettings(CommonSettings settings) {
			var fullPath = Path.Combine(Application.persistentDataPath, _relativeFilePath);
			var content = JsonConvert.SerializeObject(settings);
			File.WriteAllText(fullPath, content);
		}
	}
}
using System.IO;
using Core.Client.Models;
using Core.Client.Shared;
using Newtonsoft.Json;

namespace Core.Client.ConsoleClient.Shared {
	public sealed class FileSettingsSource : ISettingsSource {
		readonly string _relativeFilePath;

		public FileSettingsSource(string relativeFilePath = "settings.json") {
			_relativeFilePath = relativeFilePath;
		}

		public CommonSettings GetLocalSettings() {
			if ( !File.Exists(_relativeFilePath) ) {
				return new CommonSettings();
			}
			var content = File.ReadAllText(_relativeFilePath);
			return JsonConvert.DeserializeObject<CommonSettings>(content);
		}

		public void UpdateSettings(CommonSettings settings) {
			var content = JsonConvert.SerializeObject(settings);
			File.WriteAllText(_relativeFilePath, content);
		}
	}
}
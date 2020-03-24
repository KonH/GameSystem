using Core.Client.Models;

namespace Core.Client.Shared {
	public interface ISettingsSource {
		CommonSettings GetLocalSettings();
		void UpdateSettings(CommonSettings settings);
	}
}
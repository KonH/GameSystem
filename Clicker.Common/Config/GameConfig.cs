using Core.Common.Config;

namespace Clicker.Common.Config {
	public sealed class GameConfig : IConfig {
		public ConfigVersion  Version  { get; set; } = new ConfigVersion();
		public ResourceConfig Resource { get; set; } = new ResourceConfig();
		public UpgradeConfig  Upgrade  { get; set; } = new UpgradeConfig();
	}
}
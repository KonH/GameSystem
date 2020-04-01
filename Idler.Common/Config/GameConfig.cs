using Core.Common.Config;

namespace Idler.Common.Config {
	public sealed class GameConfig : IConfig {
		public ConfigVersion  Version  { get; set; } = new ConfigVersion();
		public ResourceConfig Resource { get; set; } = new ResourceConfig();
		public TimeConfig     Time     { get; set; } = new TimeConfig();
	}
}
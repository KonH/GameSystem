using Core.Service.Model;
using Core.Service.Queue;
using Core.Service.Shared;
using Idler.Common.Command;
using Idler.Common.Config;
using Idler.Common.State;

namespace Idler.Common.Watcher {
	public sealed class ResourceUpdateWatcher : IUpdateWatcher<GameConfig, GameState> {
		readonly ITimeProvider _timeProvider;

		public ResourceUpdateWatcher(ITimeProvider timeProvider) {
			_timeProvider = timeProvider;
		}

		public void TryAddCommands(UserId userId, GameConfig config, GameState state, CommandSet<GameConfig, GameState> commands) {
			var lastDate        = state.Time.LastDate;
			var curDate         = _timeProvider.UtcNow;
			var totalResources  = 0;
			var tickInterval    = config.Time.TickInterval;
			var resourcePerTick = config.Resource.ResourceByTick;
			while ( (curDate.ToUnixTimeSeconds() - lastDate.ToUnixTimeSeconds()) >= tickInterval ) {
				lastDate       =  lastDate.AddSeconds(tickInterval);
				totalResources += resourcePerTick;
			}
			if ( totalResources > 0 ) {
				commands.Add(new AddResourceCommand(totalResources));
				commands.Add(new UpdateLastDateCommand(lastDate));
			}
		}
	}
}
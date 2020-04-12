using Core.Client.UnityClient;
using Core.Service.Shared;
using Idler.Common.Config;
using Idler.Common.State;
using UnityEngine.UI;

namespace Idler.UnityClient.UpdateHandler {
    public sealed class IncomeHandler : IUpdateHandler<GameConfig, GameState> {
        readonly Slider        _slider;
        readonly ITimeProvider _time;

        public IncomeHandler(Slider slider, ITimeProvider time) {
            _slider = slider;
            _time   = time;
        }

        public void Update(GameConfig config, GameState state) {
            var curDate  = _time.UtcNow;
            var lastDate = state.Time.LastDate;
            var interval = config.Time.TickInterval;
            var normalizedValue = (curDate - lastDate).TotalSeconds / interval;
            _slider.value = (float)normalizedValue;
        }
    }
}

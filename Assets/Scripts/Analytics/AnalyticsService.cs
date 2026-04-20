using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ModularFW.Core.Locator;
using ModularFW.Core.Logging;

namespace ModularFW.Core.Analytics
{
    /// <summary>
    /// Lightweight analytics service. Tracks game events, session length, and errors via
    /// GameLogger. Extend by forwarding TrackEvent to a backend (Firebase, Unity Analytics)
    /// without changing call sites.
    /// </summary>
    public class AnalyticsService : IService, ModularFW.Core.IAnalyticsService
    {
        public static AnalyticsService Instance => SystemLocator.Instance.AnalyticsService;
        public bool IsReady { get; private set; }

        private DateTime _sessionStart;

        public Task Initialize()
        {
            _sessionStart = DateTime.UtcNow;
            IsReady = true;
            GameLogger.Log("Analytics", $"Session started at {_sessionStart:HH:mm:ss UTC}");
            return Task.CompletedTask;
        }

        public TimeSpan GetSessionDuration() => DateTime.UtcNow - _sessionStart;

        public void TrackEvent(string eventName, Dictionary<string, object> parameters = null)
        {
            var sb = new StringBuilder(eventName);
            if (parameters != null && parameters.Count > 0)
            {
                sb.Append(" | ");
                foreach (var kv in parameters)
                    sb.Append($"{kv.Key}={kv.Value} ");
            }
            GameLogger.Log("Analytics", sb.ToString().TrimEnd());
        }

        public void TrackGameStart(string gameId)
            => TrackEvent("game_start", new Dictionary<string, object> { ["game"] = gameId, ["session_s"] = (int)GetSessionDuration().TotalSeconds });

        public void TrackGameEnd(string gameId, string result)
            => TrackEvent("game_end", new Dictionary<string, object> { ["game"] = gameId, ["result"] = result, ["session_s"] = (int)GetSessionDuration().TotalSeconds });

        public void TrackError(string message)
            => TrackEvent("error", new Dictionary<string, object> { ["message"] = message });
    }
}

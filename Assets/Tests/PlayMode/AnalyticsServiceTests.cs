using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using ModularFW.Core.Analytics;

namespace Tests.PlayMode
{
    [TestFixture]
    public class AnalyticsServiceTests
    {
        private AnalyticsService _analytics;

        [SetUp]
        public async Task SetUp()
        {
            _analytics = new AnalyticsService();
            await _analytics.Initialize();
        }

        [Test]
        public void Initialize_SetsIsReady()
        {
            Assert.IsTrue(_analytics.IsReady);
        }

        [Test]
        public void GetSessionDuration_IsNonNegative()
        {
            Assert.GreaterOrEqual(_analytics.GetSessionDuration().TotalSeconds, 0);
        }

        [Test]
        public void TrackEvent_WithNoParameters_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _analytics.TrackEvent("test_event"));
        }

        [Test]
        public void TrackEvent_WithParameters_DoesNotThrow()
        {
            var parameters = new Dictionary<string, object>
            {
                ["level"] = 1,
                ["result"] = "win"
            };
            Assert.DoesNotThrow(() => _analytics.TrackEvent("game_end", parameters));
        }

        [Test]
        public void TrackGameStart_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _analytics.TrackGameStart("tictactoe"));
        }

        [Test]
        public void TrackGameEnd_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _analytics.TrackGameEnd("tictactoe", "win"));
        }

        [Test]
        public void TrackError_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _analytics.TrackError("something went wrong"));
        }

        [Test]
        public void SessionDuration_IncreasesAfterDelay()
        {
            var before = _analytics.GetSessionDuration();
            System.Threading.Thread.Sleep(10);
            var after = _analytics.GetSessionDuration();
            Assert.Greater(after.TotalMilliseconds, before.TotalMilliseconds);
        }
    }
}

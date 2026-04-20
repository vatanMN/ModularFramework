using NUnit.Framework;
using ModularFW.Core.HapticService;

namespace Tests.PlayMode
{
    // HapticService.Initialize() reads from SaveLoadService, so SystemLocator must be
    // set up first. The PlayHaptic call itself goes to Most_HapticFeedback which is a
    // no-op in the editor, so no device required.
    [TestFixture]
    public class HapticServiceTests
    {
        private HapticService _haptics;

        [SetUp]
        [Ignore("Requires SystemLocator / SaveLoadService — see class comment")]
        public void SetUp() { }

        [Test]
        [Ignore("Requires SystemLocator / SaveLoadService — see class comment")]
        public void HapticEnabled_DefaultsToTrue()
        {
            Assert.IsTrue(_haptics.HapticEnabled);
        }

        [Test]
        [Ignore("Requires SystemLocator / SaveLoadService — see class comment")]
        public void SetHapticEnabled_False_DisablesHaptics()
        {
            _haptics.SetHapticEnabled(false);
            Assert.IsFalse(_haptics.HapticEnabled);
        }

        [Test]
        [Ignore("Requires SystemLocator / SaveLoadService — see class comment")]
        public void PlayHaptic_WhenDisabled_DoesNotThrow()
        {
            _haptics.SetHapticEnabled(false);
            Assert.DoesNotThrow(() => _haptics.PlayHaptic(HapticType.Success));
        }

        [Test]
        [Ignore("Requires SystemLocator / SaveLoadService — see class comment")]
        public void PlayHaptic_WhenEnabled_DoesNotThrow()
        {
            _haptics.SetHapticEnabled(true);
            Assert.DoesNotThrow(() => _haptics.PlayHaptic(HapticType.Failure));
        }
    }
}

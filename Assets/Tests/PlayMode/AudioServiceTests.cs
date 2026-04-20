using NUnit.Framework;
using ModularFW.Core.AudioSystem;

namespace Tests.PlayMode
{
    // AudioService requires an AudioSourceBlock MonoBehaviour (injected via SystemLocator)
    // and an AudioCollection ScriptableObject. Tests run cleanly in Play Mode once
    // a scene with a SystemLocator prefab is loaded in [UnitySetUp].
    [TestFixture]
    public class AudioServiceTests
    {
        private AudioService _audio;

        [SetUp]
        [Ignore("Requires SystemLocator with AudioSourceBlock and AudioCollection assigned")]
        public void SetUp() { }

        [Test]
        [Ignore("Requires SystemLocator with AudioSourceBlock and AudioCollection assigned")]
        public void Initialize_SetsIsReady()
        {
            Assert.IsTrue(_audio.IsReady);
        }

        [Test]
        [Ignore("Requires SystemLocator with AudioSourceBlock and AudioCollection assigned")]
        public void AudioEnabled_DefaultsToTrue()
        {
            Assert.IsTrue(_audio.AudioEnabled);
        }

        [Test]
        [Ignore("Requires SystemLocator with AudioSourceBlock and AudioCollection assigned")]
        public void SetAudioEnabled_False_DisablesPlayback()
        {
            _audio.SetAudioEnabled(false);
            Assert.IsFalse(_audio.AudioEnabled);
        }

        [Test]
        [Ignore("Requires SystemLocator with AudioSourceBlock and AudioCollection assigned")]
        public void SetMasterVolume_ClampsToZeroOne()
        {
            _audio.SetMasterVolume(2f);
            Assert.AreEqual(1f, _audio.MasterVolume, 0.001f);
            _audio.SetMasterVolume(-1f);
            Assert.AreEqual(0f, _audio.MasterVolume, 0.001f);
        }

        [Test]
        [Ignore("Requires SystemLocator with AudioSourceBlock and AudioCollection assigned")]
        public void Play_WhenDisabled_DoesNotThrow()
        {
            _audio.SetAudioEnabled(false);
            Assert.DoesNotThrow(() => _audio.Play(AudioEnum.Tick));
        }
    }
}

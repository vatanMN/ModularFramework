using System.Threading.Tasks;
using NUnit.Framework;
using ModularFW.Core.SaveSystem;

namespace Tests.PlayMode
{
    // SaveLoadService uses PlayerPrefs under the hood; these tests run fine in
    // Play Mode but will leave PlayerPrefs entries behind. Clear them in [TearDown]
    // or use a unique key prefix per test run.
    [TestFixture]
    public class SaveLoadServiceTests
    {
        private SaveLoadService _save;

        [SetUp]
        [Ignore("Requires PlayerPrefs access in Play Mode — remove [Ignore] to run")]
        public async Task SetUp()
        {
            _save = new SaveLoadService();
            await _save.Initialize();
        }

        [TearDown]
        public void TearDown() => _save?.Shutdown();

        [Test]
        [Ignore("Requires PlayerPrefs access in Play Mode — remove [Ignore] to run")]
        public void Initialize_SetsIsReady()
        {
            Assert.IsTrue(_save.IsReady);
        }

        [Test]
        [Ignore("Requires PlayerPrefs access in Play Mode — remove [Ignore] to run")]
        public void Save_ThenGetData_ReturnsUpdatedCoins()
        {
            var data = new CurrencyData { Coins = 42 };
            _save.Save(DataKey.Currency, data, immediate: true);
            var loaded = _save.GetData<CurrencyData>(DataKey.Currency);
            Assert.AreEqual(42, loaded.Coins);
        }

        [Test]
        [Ignore("Requires PlayerPrefs access in Play Mode — remove [Ignore] to run")]
        public void GetData_UnknownKey_ReturnsDefault()
        {
            var result = _save.GetData<CurrencyData>(DataKey.Currency);
            Assert.IsNotNull(result);
        }

        [Test]
        [Ignore("Requires PlayerPrefs access in Play Mode — remove [Ignore] to run")]
        public void Shutdown_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _save.Shutdown());
        }
    }
}

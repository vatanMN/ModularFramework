using NUnit.Framework;
using ModularFW.Core.CurrencySystem;

namespace Tests.PlayMode
{
    // These tests require SystemLocator to be initialised (SaveLoadService dependency).
    // To enable: load a scene that initialises SystemLocator before the [SetUp] runs,
    // or refactor CurrencyService to accept an ISaveLoadService in its constructor.
    [TestFixture]
    public class CurrencyServiceTests
    {
        private CurrencyService _currency;

        [SetUp]
        [Ignore("Requires SystemLocator / SaveLoadService — see class comment")]
        public void SetUp() { }

        [Test]
        [Ignore("Requires SystemLocator / SaveLoadService — see class comment")]
        public void GetCoins_ReturnsZero_AfterFreshInit()
        {
            Assert.AreEqual(0, _currency.GetCoins());
        }

        [Test]
        [Ignore("Requires SystemLocator / SaveLoadService — see class comment")]
        public void AddCoins_IncreasesBalance()
        {
            _currency.AddCoins(10);
            Assert.AreEqual(10, _currency.GetCoins());
        }

        [Test]
        [Ignore("Requires SystemLocator / SaveLoadService — see class comment")]
        public void TrySpend_WithSufficientFunds_ReturnsTrue()
        {
            _currency.AddCoins(50);
            Assert.IsTrue(_currency.TrySpend(30));
            Assert.AreEqual(20, _currency.GetCoins());
        }

        [Test]
        [Ignore("Requires SystemLocator / SaveLoadService — see class comment")]
        public void TrySpend_WithInsufficientFunds_ReturnsFalse()
        {
            _currency.AddCoins(5);
            Assert.IsFalse(_currency.TrySpend(10));
            Assert.AreEqual(5, _currency.GetCoins());
        }

        [Test]
        [Ignore("Requires SystemLocator / SaveLoadService — see class comment")]
        public void AddCoins_Zero_NoChange()
        {
            _currency.AddCoins(0);
            Assert.AreEqual(0, _currency.GetCoins());
        }
    }
}

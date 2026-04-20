using NUnit.Framework;
using ModularFW.Core.InventorySystem;

namespace Tests.PlayMode
{
    // Requires SystemLocator (SaveLoadService + ItemCollection ScriptableObject).
    [TestFixture]
    public class InventoryServiceTests
    {
        private InventoryService _inventory;

        [SetUp]
        [Ignore("Requires SystemLocator / ItemCollection — see class comment")]
        public void SetUp() { }

        [Test]
        [Ignore("Requires SystemLocator / ItemCollection — see class comment")]
        public void IsOwned_ReturnsFalse_WhenItemNeverGranted()
        {
            Assert.IsFalse(_inventory.isOwned(999));
        }

        [Test]
        [Ignore("Requires SystemLocator / ItemCollection — see class comment")]
        public void GainItem_ThenIsOwned_ReturnsTrue()
        {
            _inventory.GainItem(1, 1);
            Assert.IsTrue(_inventory.isOwned(1));
        }

        [Test]
        [Ignore("Requires SystemLocator / ItemCollection — see class comment")]
        public void GainItem_AccumulatesCount()
        {
            _inventory.GainItem(1, 3);
            _inventory.GainItem(1, 2);
            Assert.AreEqual(5, _inventory.GetOwnItemCount(1));
        }

        [Test]
        [Ignore("Requires SystemLocator / ItemCollection — see class comment")]
        public void GetOwnItemCount_ReturnsZero_ForUnknownItem()
        {
            Assert.AreEqual(0, _inventory.GetOwnItemCount(999));
        }
    }
}

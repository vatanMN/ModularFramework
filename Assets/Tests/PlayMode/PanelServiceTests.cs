using NUnit.Framework;
using ModularFW.Core.PanelSystem;

namespace Tests.PlayMode
{
    // PanelService.Initialize() requires a Transform parent and panel prefab lists from
    // the scene. These tests are best run with a dedicated test scene that has a
    // SystemLocator prefab and a minimal panel setup.
    [TestFixture]
    public class PanelServiceTests
    {
        private PanelService _panels;

        [SetUp]
        [Ignore("Requires panel prefabs and scene setup — see class comment")]
        public void SetUp() { }

        [Test]
        [Ignore("Requires panel prefabs and scene setup — see class comment")]
        public void Initialize_SetsIsReady()
        {
            Assert.IsTrue(_panels.IsReady);
        }

        [Test]
        [Ignore("Requires panel prefabs and scene setup — see class comment")]
        public void Show_ThenHide_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _panels.Show(PanelType.NavigationPanel));
            Assert.DoesNotThrow(() => _panels.Hide(PanelType.NavigationPanel));
        }

        [Test]
        [Ignore("Requires panel prefabs and scene setup — see class comment")]
        public void Hide_NonExistentPanel_LogsWarning_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _panels.Hide(PanelType.SettingsPanel));
        }
    }
}

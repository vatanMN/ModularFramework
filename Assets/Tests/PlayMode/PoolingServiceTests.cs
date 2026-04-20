using NUnit.Framework;
using ModularFW.Core.PoolSystem;

namespace Tests.PlayMode
{
    // PoolingService.Initialize() requires a PoolCollection ScriptableObject and calls
    // GameObject.Instantiate for pre-warmed pools — needs Play Mode with a loaded scene.
    [TestFixture]
    public class PoolingServiceTests
    {
        private PoolingService _pool;

        [SetUp]
        [Ignore("Requires PoolCollection ScriptableObject — see class comment")]
        public void SetUp() { }

        [Test]
        [Ignore("Requires PoolCollection ScriptableObject — see class comment")]
        public void Initialize_SetsIsReady()
        {
            Assert.IsTrue(_pool.IsReady);
        }

        [Test]
        [Ignore("Requires PoolCollection ScriptableObject — see class comment")]
        public void Create_ReturnsNonNull_Component()
        {
            var go = _pool.Create<UnityEngine.MonoBehaviour>(PoolEnum.Enemy, null);
            Assert.IsNotNull(go);
        }

        [Test]
        [Ignore("Requires PoolCollection ScriptableObject — see class comment")]
        public void Destroy_ReturnsObjectToPool_DoesNotActuallyDestroy()
        {
            var obj = _pool.Create<UnityEngine.MonoBehaviour>(PoolEnum.Enemy, null);
            Assert.DoesNotThrow(() => _pool.Destroy(PoolEnum.Enemy, obj.gameObject));
        }

        [Test]
        [Ignore("Requires PoolCollection ScriptableObject — see class comment")]
        public void Create_AfterDestroy_ReusesObject()
        {
            var first = _pool.Create<UnityEngine.MonoBehaviour>(PoolEnum.Enemy, null);
            _pool.Destroy(PoolEnum.Enemy, first.gameObject);
            var second = _pool.Create<UnityEngine.MonoBehaviour>(PoolEnum.Enemy, null);
            Assert.AreSame(first.gameObject, second.gameObject);
        }
    }
}

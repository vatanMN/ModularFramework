using System;
using NUnit.Framework;
using ModularFW.Core.Signal;

namespace Tests.PlayMode
{
    [TestFixture]
    public class SignalBusTests
    {
        private SignalBus _bus;
        private class Ping { public int Value; }
        private class Pong { }

        [SetUp]
        public void SetUp() => _bus = new SignalBus();

        [Test]
        public void Publish_DeliversToSubscriber()
        {
            int received = 0;
            _bus.Subscribe<Ping>(p => received = p.Value);
            _bus.Publish(new Ping { Value = 42 });
            Assert.AreEqual(42, received);
        }

        [Test]
        public void Unsubscribe_StopsDelivery()
        {
            int count = 0;
            void Handler(Ping _) => count++;
            _bus.Subscribe<Ping>(Handler);
            _bus.Unsubscribe<Ping>(Handler);
            _bus.Publish(new Ping());
            Assert.AreEqual(0, count);
        }

        [Test]
        public void MultipleSubscribers_AllReceive()
        {
            int a = 0, b = 0;
            _bus.Subscribe<Ping>(_ => a++);
            _bus.Subscribe<Ping>(_ => b++);
            _bus.Publish(new Ping());
            Assert.AreEqual(1, a);
            Assert.AreEqual(1, b);
        }

        [Test]
        public void SubscribeTracked_Dispose_StopsDelivery()
        {
            int count = 0;
            var sub = _bus.SubscribeTracked<Ping>(_ => count++);
            sub.Dispose();
            _bus.Publish(new Ping());
            Assert.AreEqual(0, count);
        }

        [Test]
        public void SubscribeTracked_DisposeTwice_NoThrow()
        {
            var sub = _bus.SubscribeTracked<Ping>(_ => { });
            sub.Dispose();
            Assert.DoesNotThrow(() => sub.Dispose());
        }

        [Test]
        public void ExceptionInHandler_DoesNotBlockOtherHandlers()
        {
            int count = 0;
            _bus.Subscribe<Ping>(_ => throw new InvalidOperationException("boom"));
            _bus.Subscribe<Ping>(_ => count++);
            Assert.DoesNotThrow(() => _bus.Publish(new Ping()));
            Assert.AreEqual(1, count);
        }

        [Test]
        public void Publish_WithNoSubscribers_NoThrow()
        {
            Assert.DoesNotThrow(() => _bus.Publish(new Ping()));
        }

        [Test]
        public void DifferentSignalTypes_DoNotCrossTalk()
        {
            int pingCount = 0, pongCount = 0;
            _bus.Subscribe<Ping>(_ => pingCount++);
            _bus.Subscribe<Pong>(_ => pongCount++);
            _bus.Publish(new Ping());
            Assert.AreEqual(1, pingCount);
            Assert.AreEqual(0, pongCount);
        }

        [Test]
        public void Subscribe_NullHandler_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _bus.Subscribe<Ping>(null));
        }
    }
}

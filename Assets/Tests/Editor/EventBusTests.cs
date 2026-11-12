using IndianBikeLife.Core.Events.Core;
using NUnit.Framework;

namespace IndianBikeLife.Tests.Editor
{
    public class EventBusTests
    {
        private struct TestEvent : IGameEvent
        {
            public int Value;
            public TestEvent(int value) => Value = value;
        }

        [SetUp]
        public void Setup()
        {
            EventBus.Clear();
        }

        [TearDown]
        public void Teardown()
        {
            EventBus.Clear();
        }

        [Test]
        public void Subscribe_And_Publish_DeliversEventToSubscriber()
        {
            int receivedValue = 0;
            EventBus.Subscribe<TestEvent>(e => receivedValue = e.Value);

            EventBus.Publish(new TestEvent(42));

            Assert.AreEqual(42, receivedValue);
        }

        [Test]
        public void Unsubscribe_StopsReceivingEvents()
        {
            int callCount = 0;
            System.Action<TestEvent> handler = e => callCount++;

            EventBus.Subscribe(handler);
            EventBus.Publish(new TestEvent(1));
            Assert.AreEqual(1, callCount);

            EventBus.Unsubscribe(handler);
            EventBus.Publish(new TestEvent(2));
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void DuplicateSubscription_IsPrevented()
        {
            int callCount = 0;
            System.Action<TestEvent> handler = e => callCount++;

            EventBus.Subscribe(handler);
            EventBus.Subscribe(handler); // Duplicate

            EventBus.Publish(new TestEvent(1));
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void ExceptionInSubscriber_DoesNotCrashSubsequentSubscribers()
        {
            bool secondCalled = false;

            EventBus.Subscribe<TestEvent>(e => throw new System.Exception("Simulated error"));
            EventBus.Subscribe<TestEvent>(e => secondCalled = true);

            Assert.DoesNotThrow(() => EventBus.Publish(new TestEvent(10)));
            Assert.IsTrue(secondCalled);
        }
    }
}

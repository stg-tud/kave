using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using NUnit.Framework;
using TinyMessenger;

namespace CompletionEventBus.Tests
{
    [TestFixture]
    public class TinyMessengerEventChannelTest
    {
        private class TestEvent
        {
            public string Id { get; set; }
        }

        private IEventChannel _channelUnderTest;

        [SetUp]
        public void CreateEventChannel()
        {
            _channelUnderTest = new TinyMessengerEventChannel();
        }

        [Test]
        public void ShouldSucceedToSendEvent()
        {
            _channelUnderTest.Publish(new TestEvent {Id = "DEADBEEF"});
        }

        [Test]
        public void ShouldSucceedOnListenerRegistration()
        {
            _channelUnderTest.Subscribe<TestEvent>(e => {});
        }

        [Test]
        public void ShouldTransmitMessage()
        {
            var evt = new TestEvent { Id = "jodeldiplom!" };
            var received = false;
            _channelUnderTest.Subscribe<TestEvent>(e =>
                {
                    Assert.AreEqual(e.Id, evt.Id);
                    received = true;
                });

            _channelUnderTest.Publish(evt);

            Assert.IsTrue(received);
        }

        [Test]
        public void ShouldNotReceiveEventsOfOtherType()
        {
            _channelUnderTest.Subscribe<TestEvent>(e => Assert.Fail());
            _channelUnderTest.Publish(new Object());
        }

        [Test]
        public void DoesNotReceiveEventsOfDerivedType()
        {
            var received = false;
            _channelUnderTest.Subscribe<Object>(e => received = true);
            Assert.IsFalse(received);
        }

        [Test]
        public void ShouldNotReceiveMessagesPublishedBeforeSubscription()
        {
            var evt = new TestEvent {Id = "rambzamba"};
            var eventCount = 0;

            _channelUnderTest.Publish(evt);
            _channelUnderTest.Subscribe<TestEvent>(e => eventCount++);
            _channelUnderTest.Publish(evt);

            Assert.AreEqual(1, eventCount);
        }

        [Test]
        public void ShouldFilterMessages()
        {
            var evt1 = new TestEvent {Id = "bellcanto"};
            var evt2 = new TestEvent {Id = "growling"};
            var evt3 = new TestEvent {Id = "belting"};
            var eventCount = 0;
            _channelUnderTest.Subscribe<TestEvent>(e =>
                {
                    Assert.AreNotEqual("growling", e.Id);
                    eventCount++;
                }, e => !e.Id.Equals("growling"));

            _channelUnderTest.Publish(evt1);
            _channelUnderTest.Publish(evt2);
            _channelUnderTest.Publish(evt3);
            _channelUnderTest.Publish(evt2);

            Assert.AreEqual(2, eventCount);
        }
    }
}

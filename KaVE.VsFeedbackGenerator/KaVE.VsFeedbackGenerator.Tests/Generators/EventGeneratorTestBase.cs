using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Events;
using KaVE.VsFeedbackGenerator.MessageBus;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Generators
{
    internal abstract class EventGeneratorTestBase
    {
        private Mock<IMessageBus> _mockMessageBus;
        private IList<IDEEvent> _publishedEvents;

        private AutoResetEvent _eventReceptionLock;

        [SetUp]
        public void SetUpEventReception()
        {
            _publishedEvents = new List<IDEEvent>();
            _eventReceptionLock = new AutoResetEvent(false);
        }

        [SetUp]
        public void SetUpMessageBus()
        {
            _mockMessageBus = new Mock<IMessageBus>();
            _mockMessageBus.Setup(bus => bus.Publish(It.IsAny<IDEEvent>())).Callback(
                (IDEEvent ideEvent) => ProcessEvent(ideEvent));
        }

        private void ProcessEvent(IDEEvent ideEvent)
        {
            lock (_publishedEvents)
            {
                _publishedEvents.Add(ideEvent);
                _eventReceptionLock.Set();
            }
        }

        protected TEvent WaitForNewEvent<TEvent>(out int actualWaitMillis, int timeout = 1000) where TEvent : IDEEvent
        {
            return (TEvent) WaitForNewEvent(out actualWaitMillis, timeout);
        }

        protected IDEEvent WaitForNewEvent(out int actualWaitMillis, int timeout = 1000)
        {
            var startTime = DateTime.Now;
            var ideEvent = WaitForNewEvent(timeout);
            var endTime = DateTime.Now;
            actualWaitMillis = (int) Math.Ceiling((endTime - startTime).TotalMilliseconds);
            return ideEvent;
        }

        protected TEvent WaitForNewEvent<TEvent>(int timeout = 1000) where TEvent : IDEEvent
        {
            return (TEvent) WaitForNewEvent(timeout);
        }

        protected IDEEvent WaitForNewEvent(int timeout = 1000)
        {
            if (_eventReceptionLock.WaitOne(timeout))
            {
                return _publishedEvents.Last();
            }
            Assert.Fail("no event within {0}ms", timeout);
            return null;
        }

        protected IMessageBus TestMessageBus
        {
            get { return _mockMessageBus.Object; }
        }

        [NotNull]
        protected IEnumerable<IDEEvent> GetPublishedEvents()
        {
            return _publishedEvents.ToList();
        }

        [NotNull]
        protected TEvent GetLastPublishedEventAs<TEvent>() where TEvent : IDEEvent
        {
            var @event = _publishedEvents.Last();
            Assert.IsInstanceOf(typeof (TEvent), @event);
            return (TEvent) @event;
        }

        [NotNull]
        protected TEvent GetSinglePublishedEventAs<TEvent>() where TEvent : IDEEvent
        {
            Assert.AreEqual(1, _publishedEvents.Count);
            return GetLastPublishedEventAs<TEvent>();
        }
    }
}
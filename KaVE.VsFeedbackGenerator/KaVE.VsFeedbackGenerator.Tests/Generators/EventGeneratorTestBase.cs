using System.Collections.Generic;
using System.Linq;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Events;
using KaVE.VsFeedbackGenerator.MessageBus;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Generators
{
    abstract class EventGeneratorTestBase
    {
        private Mock<IMessageBus> _mockMessageBus;
        private IList<IDEEvent> _publishedEvents;

        [SetUp]
        public void SetUpMessageBus()
        {
            _publishedEvents = new List<IDEEvent>();
            _mockMessageBus = new Mock<IMessageBus>();
            _mockMessageBus.Setup(bus => bus.Publish(It.IsAny<IDEEvent>())).Callback(
                (IDEEvent ideEvent) => _publishedEvents.Add(ideEvent));
        }

        protected IMessageBus TestMessageBus
        {
            get
            {
                return _mockMessageBus.Object;
            }
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
            Assert.IsInstanceOf(typeof(TEvent), @event);
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

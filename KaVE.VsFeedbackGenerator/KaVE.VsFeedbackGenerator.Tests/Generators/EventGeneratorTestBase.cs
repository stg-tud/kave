/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * Contributors:
 *    - Sven Amann
 */

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Events;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Tests.Utils;
using KaVE.VsFeedbackGenerator.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Generators
{
    internal abstract class EventGeneratorTestBase
    {
        private IList<IDEEvent> _publishedEvents;
        private AutoResetEvent _eventReceptionLock;
        private Mock<IMessageBus> _mockMessageBus;
        protected TestIDESession TestIDESession;
        protected TestDateUtils TestDateUtils { get; private set; }

        [SetUp]
        public void SetUpEventReception()
        {
            TestIDESession = new TestIDESession();
            TestDateUtils = new TestDateUtils();
            Registry.RegisterComponent<IDateUtils>(TestDateUtils);

            _publishedEvents = new List<IDEEvent>();
            _eventReceptionLock = new AutoResetEvent(false);
            _mockMessageBus = new Mock<IMessageBus>();
            _mockMessageBus.Setup(bus => bus.Publish(It.IsAny<IDEEvent>())).Callback(
                (IDEEvent ideEvent) => ProcessEvent(ideEvent));
        }

        [TearDown]
        public void TearDown()
        {
            Registry.Clear();
        }

        private void ProcessEvent(IDEEvent ideEvent)
        {
            lock (_publishedEvents)
            {
                _publishedEvents.Add(ideEvent);
                _eventReceptionLock.Set();
            }
        }

        protected IMessageBus TestMessageBus
        {
            get { return _mockMessageBus.Object; }
        }

        protected void AssertNoEvent()
        {
            CollectionAssert.IsEmpty(_publishedEvents);
        }

        [NotNull]
        protected IEnumerable<IDEEvent> GetPublishedEvents()
        {
            return _publishedEvents.ToList();
        }

        [NotNull]
        protected TEvent GetLastPublished<TEvent>() where TEvent : IDEEvent
        {
            var @event = _publishedEvents.Last();
            Assert.IsInstanceOf(typeof (TEvent), @event);
            return (TEvent) @event;
        }

        [NotNull]
        protected TEvent GetSinglePublished<TEvent>() where TEvent : IDEEvent
        {
            Assert.AreEqual(1, _publishedEvents.Count, "expected single published event, found multiple");
            return GetLastPublished<TEvent>();
        }

        protected void DropAllEvents()
        {
            _publishedEvents.Clear();
        }
    }
}
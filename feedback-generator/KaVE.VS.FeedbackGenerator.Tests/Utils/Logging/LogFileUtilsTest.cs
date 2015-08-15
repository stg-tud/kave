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
 */

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Util;
using KaVE.Commons.Model.Events;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.VS.FeedbackGenerator.MessageBus;
using KaVE.VS.FeedbackGenerator.Utils.Logging;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Utils.Logging
{
    internal class LogFileUtilsTest
    {
        private ILogManager _sourceLogFileManager;
        private IMessageBus _messageBus;

        private readonly IList<IDEEvent> _receivedEvents = new List<IDEEvent>();

        [SetUp]
        public void SetUp()
        {
            var eventsToday = new List<IDEEvent>
            {
                IDEEventTestFactory.SomeEvent(DateTime.Today.AddHours(0)),
                IDEEventTestFactory.SomeEvent(DateTime.Today.AddHours(1)),
                IDEEventTestFactory.SomeEvent(DateTime.Today.AddHours(2))
            };

            var eventsYesterday = new List<IDEEvent>
            {
                IDEEventTestFactory.SomeEvent(DateTime.Today.AddDays(-1).AddHours(0)),
                IDEEventTestFactory.SomeEvent(DateTime.Today.AddDays(-1).AddHours(1)),
                IDEEventTestFactory.SomeEvent(DateTime.Today.AddDays(-1).AddHours(2))
            };

            var mockLogToday = new Mock<ILog>();
            mockLogToday.Setup(m => m.ReadAll()).Returns(eventsToday);

            var mockLogYesterday = new Mock<ILog>();
            mockLogYesterday.Setup(m => m.ReadAll()).Returns(eventsYesterday);

            var mockLogManager = new Mock<ILogManager>();
            mockLogManager.Setup(m => m.Logs).Returns(new[] {mockLogToday.Object, mockLogYesterday.Object});

            _sourceLogFileManager = mockLogManager.Object;

            var mockMessageBus = new Mock<IMessageBus>();
            mockMessageBus.Setup(m => m.Publish(It.IsAny<IDEEvent>()))
                          .Callback<IDEEvent>(e => _receivedEvents.Add(e));

            _messageBus = mockMessageBus.Object;
        }

        [Test]
        public void AllResubmittedEventsArrive()
        {
            var count = LogFileUtils.ResubmitLogs(_sourceLogFileManager, _messageBus);
            Assert.AreEqual(6, count);
        }

        [Test]
        public void ResubmittedEventsDontChange()
        {
            var hashes = _sourceLogFileManager.Logs.SelectMany(l => l.ReadAll()).Select(e => e.GetHashCode()).ToList();
            Assert.AreEqual(6, hashes.Count);
            LogFileUtils.ResubmitLogs(_sourceLogFileManager, _messageBus);
            _receivedEvents.ForEach(e => hashes.Remove(e.GetHashCode()));
            Assert.AreEqual(0, hashes.Count);
        }
    }
}
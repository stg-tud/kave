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

using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Util;
using KaVE.TestUtils.Model.Events;
using KaVE.VsFeedbackGenerator.Generators;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils.Logging;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Generators
{
    [TestFixture]
    internal class EventLoggerTest
    {
        private class TestMessageBus : IMessageBus
        {
            private readonly IList<Action<object>> _receivers = new List<Action<object>>();

            public void Publish<TMessage>(TMessage evt) where TMessage : class
            {
                _receivers.ForEach(r => r.Invoke(evt));
            }

            public void Subscribe<TMessage>(Action<TMessage> action, Func<TMessage, bool> filter = null)
                where TMessage : class
            {
                _receivers.Add(o => action((TMessage) o));
            }
        }

        [Test]
        public void ShouldNotFailIfLoggingFails()
        {
            var evt1 = IDEEventTestFactory.SomeEvent();
            var mockLogManager = new Mock<ILogManager>();
            mockLogManager.Setup(lm => lm.CurrentLog.Append(evt1)).Throws<IOException>();

            var testMessageBus = new TestMessageBus();
            // ReSharper disable once ObjectCreationAsStatement
            new EventLogger(testMessageBus, mockLogManager.Object);

            // first event goes to buffer
            testMessageBus.Publish(evt1);
            testMessageBus.Publish(evt1);
        }
    }
}
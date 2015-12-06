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
using System.Windows.Forms;
using KaVE.Commons.Model.Events;
using KaVE.VS.FeedbackGenerator.Generators.Activity;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators.Activity
{
    internal class MouseActivityEventGeneratorTest : EventGeneratorTestBase
    {
        private Mock<IKaVEMouseEvents> _mouseEventsMock;

        [SetUp]
        public void Setup()
        {
            _mouseEventsMock = new Mock<IKaVEMouseEvents>();

            // ReSharper disable once ObjectCreationAsStatement
            new MouseActivityEventGenerator(TestRSEnv, TestMessageBus, TestDateUtils, _mouseEventsMock.Object);
        }

        [Test]
        public void ShouldFireOnMove()
        {
            Raise(events => events.MouseMove += null);
            var actualEvent = GetSinglePublished<ActivityEvent>();
            Assert.AreEqual(IDEEvent.Trigger.Click, actualEvent.TriggeredBy);
        }

        [Test]
        public void ShouldFireOnClick()
        {
            Raise(events => events.MouseClick += null);
            var actualEvent = GetSinglePublished<ActivityEvent>();
            Assert.AreEqual(IDEEvent.Trigger.Click, actualEvent.TriggeredBy);
        }

        [Test]
        public void ShouldFireOnWheel()
        {
            Raise(events => events.MouseWheel += null);
            var actualEvent = GetSinglePublished<ActivityEvent>();
            Assert.AreEqual(IDEEvent.Trigger.Click, actualEvent.TriggeredBy);
        }

        private void Raise(Action<IKaVEMouseEvents> mouseEvent)
        {
            _mouseEventsMock.Raise(
                mouseEvent,
                new MouseEventArgs(MouseButtons.None, 1, 0, 0, 0));
        }
    }
}
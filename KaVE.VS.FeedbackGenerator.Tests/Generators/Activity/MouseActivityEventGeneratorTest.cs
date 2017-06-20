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
using KaVE.VS.FeedbackGenerator.Generators.VisualStudio;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators.Activity
{
    internal class MouseActivityEventGeneratorTest : EventGeneratorTestBase
    {
        private Mock<IKaVEMouseEvents> _mouseEventsMock;
        private bool _hasFocus;

        [SetUp]
        public void Setup()
        {
            _mouseEventsMock = new Mock<IKaVEMouseEvents>();

            _hasFocus = true;
            var focusHelper = Mock.Of<IFocusHelper>();
            Mock.Get(focusHelper).Setup(fh => fh.IsCurrentApplicationActive()).Returns(() => _hasFocus);

            // ReSharper disable once ObjectCreationAsStatement
            new MouseActivityEventGenerator(
                TestRSEnv,
                TestMessageBus,
                TestDateUtils,
                _mouseEventsMock.Object,
                focusHelper,
                TestThreading);
        }

        [Test]
        public void ShouldFireOnMove()
        {
            MoveMouse();
            MakeUserInactive();
            MoveMouse();

            var actualEvent = GetSinglePublished<ActivityEvent>();
            Assert.AreEqual(EventTrigger.Click, actualEvent.TriggeredBy);
        }

        [Test]
        public void ShouldFireOnClick()
        {
            Raise(events => events.MouseClick += null);
            MakeUserInactive();
            Raise(events => events.MouseMove += null);

            var actualEvent = GetSinglePublished<ActivityEvent>();
            Assert.AreEqual(EventTrigger.Click, actualEvent.TriggeredBy);
        }

        [Test]
        public void ShouldFireOnWheel()
        {
            Raise(events => events.MouseWheel += null);
            MakeUserInactive();
            Raise(events => events.MouseMove += null);

            var actualEvent = GetSinglePublished<ActivityEvent>();
            Assert.AreEqual(EventTrigger.Click, actualEvent.TriggeredBy);
        }

        [Test]
        public void ShouldNotFireMultipleEventsInARow()
        {
            Raise(events => events.MouseWheel += null);
            Raise(events => events.MouseWheel += null);
            Raise(events => events.MouseWheel += null);
            MakeUserInactive();
            Raise(events => events.MouseMove += null);

            var actualEvent = GetSinglePublished<ActivityEvent>();
            Assert.AreEqual(EventTrigger.Click, actualEvent.TriggeredBy);
        }

        [Test]
        public void CorrectStartAndDurationIsRegistered()
        {
            var now = TestDateUtils.Now;
            MoveMouse();
            TimePassed(1);
            MoveMouse();
            TimePassed(1);
            MoveMouse();
            MakeUserInactive();
            MoveMouse();

            var actual = GetSinglePublished<ActivityEvent>();
            Assert.AreEqual(now, actual.TriggeredAt);
            // ReSharper disable once PossibleInvalidOperationException
            Assert.AreEqual(2, actual.Duration.Value.Seconds);
        }

        [Test]
        public void ActivityOutsideIDEIsNotTracked()
        {
            var now = TestDateUtils.Now;
            MoveMouse();
            TimePassed(1);
            MoveMouse();
            _hasFocus = false;
            TimePassed(1);
            MoveMouse();
            MakeUserInactive();
            MoveMouse();

            var actual = GetSinglePublished<ActivityEvent>();
            Assert.AreEqual(now, actual.TriggeredAt);
            // ReSharper disable once PossibleInvalidOperationException
            Assert.AreEqual(1, actual.Duration.Value.Seconds);
        }

        [Test]
        public void MultipleEventsAreCorrect()
        {
            var start1 = TestDateUtils.Now;
            MoveMouse();
            TimePassed(1);
            MoveMouse();
            MakeUserInactive();
            var start2 = TestDateUtils.Now;
            MoveMouse();
            TimePassed(2);
            MoveMouse();
            MakeUserInactive();
            MoveMouse();

            var actuals = GetPublishedEvents().GetEnumerator();
            actuals.MoveNext();

            var actual = actuals.Current;
            Assert.AreEqual(start1, actual.TriggeredAt);
            // ReSharper disable once PossibleInvalidOperationException
            Assert.AreEqual(1, actual.Duration.Value.Seconds);

            actuals.MoveNext();
            actual = actuals.Current;
            Assert.AreEqual(start2, actual.TriggeredAt);
            // ReSharper disable once PossibleInvalidOperationException
            Assert.AreEqual(2, actual.Duration.Value.Seconds);

            actuals.MoveNext();
            Assert.Null(actuals.Current);
        }

        private void Raise(Action<IKaVEMouseEvents> mouseEvent)
        {
            _mouseEventsMock.Raise(
                mouseEvent,
                new MouseEventArgs(MouseButtons.None, 1, 0, 0, 0));
        }

        private void MakeUserInactive()
        {
            TestDateUtils.Now += MouseActivityEventGenerator.InactivitySpanToBreakActivityPeriod +
                                 TimeSpan.FromMilliseconds(1);
        }

        private void TimePassed(int spanInSeconds)
        {
            TestDateUtils.Now += TimeSpan.FromSeconds(spanInSeconds);
        }

        private void MoveMouse()
        {
            Raise(events => events.MouseMove += null);
        }
    }
}
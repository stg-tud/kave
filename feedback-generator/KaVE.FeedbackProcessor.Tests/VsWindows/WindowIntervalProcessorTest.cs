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
using KaVE.Commons.Model.Names.VisualStudio;
using KaVE.FeedbackProcessor.Activities.Model;
using KaVE.FeedbackProcessor.Tests.Activities.Intervals;
using KaVE.FeedbackProcessor.VsWindows;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.VsWindows
{
    [TestFixture]
    internal class WindowIntervalProcessorTest : IntervalProcessorTest<WindowIntervalProcessor, string>
    {
        protected override WindowIntervalProcessor CreateProcessor()
        {
            return new WindowIntervalProcessor();
        }

        [Test]
        public void Interval()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, ":some window:", 1));

            AssertStream(SomeDay, Interval(0, ":some window:", 1));
        }

        [Test]
        public void Intervals()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, ":one window:", 1),
                SomeEvent(1, ":other window:", 1));

            AssertStream(
                SomeDay,
                Interval(0, ":one window:", 1),
                Interval(1, ":other window:", 1));
        }

        [Test]
        public void IntervalsWithGap()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, ":one window:", 1),
                SomeEvent(2, ":other window:", 1));

            AssertStream(
                SomeDay,
                Interval(0, ":one window:", 2),
                Interval(2, ":other window:", 1));
        }

        [Test]
        public void StayInWindow()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, ":a window:", 1),
                SomeEvent(2, ":a window:", 1));

            AssertStream(SomeDay, Interval(0, ":a window:", 3));
        }

        [Test]
        public void Concurrent()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, ":one window:", 2),
                SomeEvent(1, ":another window:", 2));

            AssertStream(
                SomeDay,
                Interval(0, ":one window:", 1),
                Interval(1, ":another window:", 2));
        }

        [Test]
        public void LeaveIDE()
        {
            WhenStreamIsProcessed(
                LeaveEvent(0, ":any window:", 1));

            AssertStream(SomeDay, Interval(0, WindowIntervalProcessor.OutsideIDEIntervalId, 1));
        }

        [Test]
        public void EnterIDE()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, ":some window:", 2),
                EnterEvent(5, ":any window:", 1));

            AssertStream(
                SomeDay,
                Interval(0, ":some window:", 2),
                Interval(2, WindowIntervalProcessor.OutsideIDEIntervalId, 3),
                Interval(5, ":any window:", 1));
        }

        [Test]
        public void LeaveAndEnterIDE()
        {
            WhenStreamIsProcessed(
                LeaveEvent(0, ":some window:", 1),
                EnterEvent(6, ":other window:", 1));

            AssertStream(
                SomeDay,
                Interval(0, WindowIntervalProcessor.OutsideIDEIntervalId, 6),
                Interval(6, ":other window:", 1));
        }

        private ActivityEvent LeaveEvent(int triggerTimeOffset, string windowTitle, int eventDuration)
        {
            var someEvent = SomeEvent(triggerTimeOffset, windowTitle, eventDuration);
            someEvent.Activity = Activity.LeaveIDE;
            return someEvent;
        }

        private ActivityEvent EnterEvent(int triggerTimeOffset, string windowTitle, int eventDuration)
        {
            var someEvent = SomeEvent(triggerTimeOffset, windowTitle, eventDuration);
            someEvent.Activity = Activity.EnterIDE;
            return someEvent;
        }

        private ActivityEvent SomeEvent(int offsetInSeconds, string windowTitle, int durationInSeconds)
        {
            var activityEvent = SomeEvent(offsetInSeconds, durationInSeconds);
            activityEvent.ActiveWindow = WindowName.Get(windowTitle);
            return activityEvent;
        }
    }
}
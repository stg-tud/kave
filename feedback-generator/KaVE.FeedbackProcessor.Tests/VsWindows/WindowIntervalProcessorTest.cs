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

            AssertIntervals(
                Interval(0, ":some window:", 1));
        }

        [Test]
        public void Intervals()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, ":one window:", 1),
                SomeEvent(1, ":other window:", 1));

            AssertIntervals(
                Interval(0, ":one window:", 1),
                Interval(1, ":other window:", 1));
        }

        private ActivityEvent SomeEvent(int triggerTimeOffset, string windowTitle, int eventDuration)
        {
            return new ActivityEvent
            {
                TriggeredAt = SomeDateTime.AddSeconds(triggerTimeOffset),
                ActiveWindow = WindowName.Get(windowTitle),
                Duration = TimeSpan.FromSeconds(eventDuration)
            };
        }
    }
}
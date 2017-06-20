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
using KaVE.FeedbackProcessor.Cleanup.Heuristics;
using KaVE.FeedbackProcessor.Tests.TestUtils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Cleanup.Heuristics
{
    [TestFixture]
    internal class ConcurrentEventHeuristicTest
    {
        [Test]
        public void HaveSimiliarEventTimeTest()
        {
            var eventTime1 = DateTimeFactory.SomeWorkingHoursDateTime();
            var eventTime2 = eventTime1.Add(ConcurrentEventHeuristic.EventTimeDifference);

            Assert.IsTrue(ConcurrentEventHeuristic.AreSimilar(eventTime1, eventTime2));
        }

        [Test]
        public void ShouldReturnFalseForNonSimilarEventTimes()
        {
            var eventTime1 = DateTimeFactory.SomeWorkingHoursDateTime();
            var eventTime2 = eventTime1.Add(ConcurrentEventHeuristic.EventTimeDifference + TimeSpan.FromTicks(1));

            Assert.IsFalse(ConcurrentEventHeuristic.AreSimilar(eventTime1, eventTime2));
        }

        [TestCase("TextControl.Left",true)]
        [TestCase("TextControl.Right",true)]
        [TestCase("TextControl.Up",true)]
        [TestCase("TextControl.Down",true)]
        [TestCase("TextControl.Backspace",true)]
        [TestCase("TextControl.Enter",true)]
        [TestCase("TextControl.Up.Selection", true)]
        [TestCase("TextControl.Down.Selection", true)]
        [TestCase("TextControl.Left.Selection", true)]
        [TestCase("TextControl.Right.Selection", true)]
        [TestCase("TextControl.Cut",false)]
        [TestCase("Cut",false)]
        public void ShouldDetectIgnorableTextControlEvents(string commandId, bool expected)
        {
            Assert.AreEqual(expected,ConcurrentEventHeuristic.IsIgnorableTextControlCommand(commandId));
        }
    }
}
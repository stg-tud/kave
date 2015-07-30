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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.Commons.Utils.Collections;
using KaVE.VS.FeedbackGenerator.MessageBus;
using KaVE.VS.Statistics.Calculators;
using KaVE.VS.Statistics.StatisticListing;
using KaVE.VS.Statistics.Statistics;
using KaVE.VS.Statistics.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.Statistics.Tests.Calculators
{
    [TestFixture]
    public class CompletionCalculatorTest : CalculatorTest
    {
        private static readonly CompletionEvent AppliedCompletion = new CompletionEvent
        {
            TerminatedState = TerminationState.Applied,
            Duration = new TimeSpan(0, 3, 30)
        };

        private static readonly CompletionEvent CancelledCompletion = new CompletionEvent
        {
            TerminatedState = TerminationState.Cancelled,
            Duration = new TimeSpan(0, 1, 30)
        };

        public CompletionCalculatorTest()
            : base(typeof (CompletionCalculatorTestImplementation), typeof (CompletionStatistic), new CompletionEvent()) {}

        protected override bool IsNewStatistic(IStatistic statistic)
        {
            var completionStatistic = statistic as CompletionStatistic;
            if (completionStatistic == null)
            {
                return false;
            }
            return completionStatistic.AverageSavedKeystrokes.CompareTo(0) == 0 &&
                   completionStatistic.AverageTimeCancelled == new TimeSpan() &&
                   completionStatistic.AverageTimeCompleted == new TimeSpan() &&
                   completionStatistic.SavedKeystrokes == 0 &&
                   completionStatistic.TotalCancelled == 0 &&
                   completionStatistic.TotalCompleted == 0 &&
                   completionStatistic.TotalCompletions == 0 &&
                   completionStatistic.TotalProposals == 0 &&
                   completionStatistic.TotalTime == new TimeSpan() &&
                   completionStatistic.TotalTimeCancelled == new TimeSpan() &&
                   completionStatistic.TotalTimeCompleted == new TimeSpan();
        }

        [Test]
        public void TotalTest()
        {
            var events = new[]
            {
                AppliedCompletion,
                CancelledCompletion,
                AppliedCompletion
            };

            const int expectedTotal = 3;
            var expectedTime = new TimeSpan(0, 8, 30);

            var actualStatistic = (CompletionStatistic) ListingMock.Object.GetStatistic(StatisticType);

            Publish(events);

            Assert.AreEqual(expectedTotal, actualStatistic.TotalCompletions);
            Assert.AreEqual(expectedTime, actualStatistic.TotalTime);
        }

        [Test]
        public void CompletedTest()
        {
            var events = new[]
            {
                CancelledCompletion,
                AppliedCompletion,
                CancelledCompletion,
                AppliedCompletion,
                CancelledCompletion
            };

            const int expectedTotalCompleted = 2;
            var expectedTotalTimeCompleted = new TimeSpan(0, 7, 00);

            var actualStatistic = (CompletionStatistic) ListingMock.Object.GetStatistic(StatisticType);

            Publish(events);

            Assert.AreEqual(expectedTotalCompleted, actualStatistic.TotalCompleted);
            Assert.AreEqual(expectedTotalTimeCompleted, actualStatistic.TotalTimeCompleted);
        }

        [Test]
        public void CancelledTest()
        {
            var events = new[]
            {
                AppliedCompletion,
                CancelledCompletion,
                AppliedCompletion,
                CancelledCompletion,
                AppliedCompletion
            };

            const int expectedTotalCancelled = 2;
            var expectedTotalTimeCancelled = new TimeSpan(0, 3, 00);

            var actualStatistic = (CompletionStatistic) ListingMock.Object.GetStatistic(StatisticType);

            Publish(events);

            Assert.AreEqual(expectedTotalCancelled, actualStatistic.TotalCancelled);
            Assert.AreEqual(expectedTotalTimeCancelled, actualStatistic.TotalTimeCancelled);
        }

        [TestCase("var", "[value-type-identifier] variablename", 9),
         TestCase("",
             "set get static [System.Object, mscorlib, 4.0.0.0] [TestProject.Class1, TestProject, 0.0.0.0].Foo()", 3),
         TestCase("Name", "[value-type-identifier] Name.varName", 8),
         TestCase("Gen",
             "static [System.Collections.Generic.List`1[[T -> System.Int32, mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0] [Katas.PrimeFactors, Katas, 1.0.0.0].Generate([System.Int32, mscorlib, 4.0.0.0] i)",
             7),
         TestCase("Re",
             "[TestProject.Tests.RegexTest, TestProject, 0.0.0.0] [TestProject.Tests.RegexTest, TestProject, 0.0.0.0]..ctor()",
             9), TestCase("Li", "System.Collections.Generic.List`1[[T -> T]], mscorlib, 4.0.0.0", 4),
         TestCase("Co", "get [System.Int32, mscore, 4.0.0.0] [MyClass, MyAssembly, 1.2.3.4].Count", 3),
         TestCase("CodeCompletion.Model.N", "CodeCompletion.Model.Names, 1.0.0.0", 4),
         TestCase("ref", "ref [System.Int32, mscore, 4.0.0.0] referenceParameter", 15)]
        public void ShouldCalculateSavedKeystrokesCorrectly(string prefix,
            string fullIdentifier,
            int expectedTotal)
        {
            var testEvent = new CompletionEvent
            {
                TerminatedState = TerminationState.Applied,
                Prefix = prefix,
                Selections =
                    Lists.NewList<IProposalSelection>(
                        new ProposalSelection(new Proposal {Name = Name.Get(fullIdentifier)}))
            };

            var actualStatistic = (CompletionStatistic) ListingMock.Object.GetStatistic(StatisticType);

            Publish(testEvent);

            Assert.AreEqual(expectedTotal, actualStatistic.SavedKeystrokes);
        }

        [Test]
        public void EventCallWithWrongEventTypeExceptionHandlingTest()
        {
            Publish(new TestIDEEvent());

            ListingMock.Verify(l => l.Update(It.IsAny<IStatistic>()), Times.Never);
        }

        [Test]
        public void IsGenericTypeIdentifierTest()
        {
            Assert.True(
                CompletionCalculator.IsGenericTypeIdentifier(
                    "KaVE.BP.Achievements.Util.Listing`2[[TKey -> TKey],[TValue -> TValue]], KaVE.BP.Achievements, 1.0.0.0"));
            Assert.False(
                CompletionCalculator.IsGenericTypeIdentifier(
                    "KaVE.BP.Achievements.Util.Listing`2[->], KaVE.BP.Achievements, 1.0.0.0"));
        }

        [Test]
        public void IsMethodIdentifierTest()
        {
            Assert.True(
                CompletionCalculator.IsMethodIdentifier("[TestProject.Tests.RegexTest, TestProject, 0.0.0.0]..ctor()"));
            Assert.False(
                CompletionCalculator.IsMethodIdentifier("[TestProject.Tests.RegexTest, TestProject, 0.0.0.0]..ctor"));
        }

        [Test]
        public void IsPropertyIdentifierTest()
        {
            Assert.True(
                CompletionCalculator.IsPropertyIdentifier(
                    "set get static [System.Object, mscorlib, 4.0.0.0] [TestProject.Class1, TestProject, 0.0.0.0].Foo()"));
            Assert.False(
                CompletionCalculator.IsPropertyIdentifier(
                    "static [System.Object, mscorlib, 4.0.0.0] [TestProject.Class1, TestProject, 0.0.0.0].Foo()"));
        }

        public class CompletionCalculatorTestImplementation : CompletionCalculator
        {
            public CompletionCalculatorTestImplementation(IStatisticListing statisticListing,
                IMessageBus messageBus,
                IErrorHandler errorHandler) : base(statisticListing, messageBus, errorHandler) {}

            protected override IDEEvent FilterEvent(IDEEvent @event)
            {
                return @event;
            }
        }
    }
}
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
using JetBrains.Reflection;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.VS.Achievements.Statistics.Calculators;
using KaVE.VS.Achievements.Statistics.Filters;
using KaVE.VS.Achievements.Statistics.Listing;
using KaVE.VS.Achievements.Statistics.Statistics;
using KaVE.VS.Achievements.Util;
using KaVE.VS.FeedbackGenerator.MessageBus;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.Achievements.Tests.Statistics.Calculators
{
    [TestFixture]
    internal class SolutionCalculatorTest : CalculatorTest
    {
        public SolutionCalculatorTest()
            : base(typeof (SolutionCalculatorTestImplementation), typeof (SolutionStatistic), new SolutionEvent()) {}

        protected override bool IsNewStatistic(IStatistic statistic)
        {
            var solutionStatistic = statistic as SolutionStatistic;
            if (solutionStatistic == null)
            {
                return false;
            }
            return solutionStatistic.SolutionsOpened == 0 &&
                   solutionStatistic.SolutionsRenamed == 0 &&
                   solutionStatistic.SolutionsClosed == 0 &&
                   solutionStatistic.SolutionItemsAdded == 0 &&
                   solutionStatistic.SolutionItemsRenamed == 0 &&
                   solutionStatistic.SolutionItemsRemoved == 0 &&
                   solutionStatistic.ProjectsAdded == 0 &&
                   solutionStatistic.ProjectsRenamed == 0 &&
                   solutionStatistic.ProjectsRemoved == 0 &&
                   solutionStatistic.ProjectItemsAdded == 0 &&
                   solutionStatistic.ProjectItemsRenamed == 0 &&
                   solutionStatistic.ProjectItemsRemoved == 0 &&
                   solutionStatistic.TestClassesCreated == 0;
        }

        private static readonly object[] ComputesStatisticsCorrectlySource =
        {
            new object[] {SolutionEvent.SolutionAction.OpenSolution, "SolutionsOpened"},
            new object[] {SolutionEvent.SolutionAction.RenameSolution, "SolutionsRenamed"},
            new object[] {SolutionEvent.SolutionAction.CloseSolution, "SolutionsClosed"},
            new object[] {SolutionEvent.SolutionAction.AddSolutionItem, "SolutionItemsAdded"},
            new object[] {SolutionEvent.SolutionAction.RenameSolutionItem, "SolutionItemsRenamed"},
            new object[] {SolutionEvent.SolutionAction.RemoveSolutionItem, "SolutionItemsRemoved"},
            new object[] {SolutionEvent.SolutionAction.AddProject, "ProjectsAdded"},
            new object[] {SolutionEvent.SolutionAction.RenameProject, "ProjectsRenamed"},
            new object[] {SolutionEvent.SolutionAction.RemoveProject, "ProjectsRemoved"},
            new object[] {SolutionEvent.SolutionAction.AddProjectItem, "ProjectItemsAdded"},
            new object[] {SolutionEvent.SolutionAction.RenameProjectItem, "ProjectItemsRenamed"},
            new object[] {SolutionEvent.SolutionAction.RemoveProjectItem, "ProjectItemsRemoved"}
        };

        public static object[] IsTestItemTestSources =
        {
            new object[] {"", false},
            new object[] {"Test.txt", false},
            new object[] {"Test.cs", true},
            new object[] {"ATest.cs", true},
            new object[] {"Test.c", true},
            new object[] {"Test.cpp", true}
        };

        public class SolutionCalculatorTestImplementation : SolutionCalculator
        {
            public SolutionCalculatorTestImplementation(IStatisticListing statisticListing,
                IMessageBus messageBus,
                IErrorHandler errorHandler) : base(statisticListing, messageBus, errorHandler) {}

            /// <summary>
            ///     Use no filter logic
            /// </summary>
            protected override IDEEvent FilterEvent(IDEEvent @event)
            {
                return @event;
            }
        }

        [Test]
        public void ComputesAddedTestClassesEventCorrectly()
        {
            var actualStatistic = (SolutionStatistic) ListingMock.Object.GetStatistic(StatisticType);
            var previousCreatedTestClasses = actualStatistic.TestClassesCreated;

            var testClassCreatedEvent = new SolutionEvent
            {
                Action = SolutionEvent.SolutionAction.AddProjectItem,
                Target = new SolutionFilter.ComponentName("Test.cs")
            };
            var someOtherClassCreatedEvent = new SolutionEvent
            {
                Action = SolutionEvent.SolutionAction.AddProjectItem,
                Target = new SolutionFilter.ComponentName("Class1.cs")
            };

            Publish(testClassCreatedEvent);
            Publish(someOtherClassCreatedEvent);

            Assert.AreEqual(previousCreatedTestClasses + 1, actualStatistic.TestClassesCreated);
        }

        [Test]
        public void ComputesRemovedTestClassesEventCorrectly()
        {
            var actualStatistic = (SolutionStatistic) ListingMock.Object.GetStatistic(StatisticType);
            var previousCreatedTestClasses = actualStatistic.TestClassesCreated;

            var testClassCreatedEvent = new SolutionEvent
            {
                Action = SolutionEvent.SolutionAction.RemoveProjectItem,
                Target = new SolutionFilter.ComponentName("Test.cs")
            };
            var someOtherClassCreatedEvent = new SolutionEvent
            {
                Action = SolutionEvent.SolutionAction.RemoveProjectItem,
                Target = new SolutionFilter.ComponentName("Class1.cs")
            };

            Publish(testClassCreatedEvent);
            Publish(someOtherClassCreatedEvent);

            Assert.AreEqual(previousCreatedTestClasses - 1, actualStatistic.TestClassesCreated);
        }

        [Test, TestCaseSource("ComputesStatisticsCorrectlySource")]
        public void ComputesStatisticsCorrectly(SolutionEvent.SolutionAction action, String testedStatistic)
        {
            var actualStatistic = (SolutionStatistic) ListingMock.Object.GetStatistic(StatisticType);
            var previousValue = (int) actualStatistic.GetFieldOrPropertyValue(testedStatistic);

            var testClassCreatedEvent = new SolutionEvent {Action = action};

            Publish(testClassCreatedEvent);

            var actualValue = (int) actualStatistic.GetFieldOrPropertyValue(testedStatistic);
            Assert.AreEqual(previousValue + 1, actualValue);
        }

        [Test]
        public void EventCallWithWrongEventTypeExceptionHandlingTest()
        {
            Publish(new StatisticCalculatorTest.TestEvent());

            ListingMock.Verify(l => l.Update(It.IsAny<IStatistic>()), Times.Never);
        }

        [Test, TestCaseSource("IsTestItemTestSources")]
        public void IsTestItemTest(string identifier, bool expected)
        {
            Assert.AreEqual(expected, SolutionCalculator.IsTestItem(identifier));
        }
    }
}
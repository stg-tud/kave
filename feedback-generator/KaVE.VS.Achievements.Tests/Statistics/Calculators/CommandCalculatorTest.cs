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

using JetBrains.Util;
using KaVE.Commons.Model.Events;
using KaVE.VS.Achievements.Statistics.Calculators;
using KaVE.VS.Achievements.Statistics.Listing;
using KaVE.VS.Achievements.Statistics.Statistics;
using KaVE.VS.Achievements.Util;
using KaVE.VS.FeedbackGenerator.MessageBus;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.Achievements.Tests.Statistics.Calculators
{
    [TestFixture]
    public class CommandCalculatorTest : CalculatorTest
    {
        public CommandCalculatorTest()
            : base(
                typeof (CommandCalculatorTestImplementation),
                typeof (CommandStatistic),
                new CommandEvent {CommandId = ""}) {}

        protected override bool IsNewStatistic(IStatistic statistic)
        {
            var commandStatistic = statistic as CommandStatistic;
            return commandStatistic != null && commandStatistic.CommandTypeValues.IsEmpty();
        }

        protected static readonly CommandEvent[] CommandEvents =
        {
            new CommandEvent {CommandId = "File.SaveSelectedItems"},
            new CommandEvent {CommandId = "View.CallHierarchy"},
            new CommandEvent {CommandId = "Edit.LineDelete"},
            new CommandEvent {CommandId = "Rename"},
            new CommandEvent {CommandId = "Edit.LineEnd"},
            new CommandEvent {CommandId = "Edit.FormatSelection"},
            new CommandEvent {CommandId = "File.OpenFile"},
            new CommandEvent {CommandId = "Edit.Paste"},
            new CommandEvent {CommandId = "Tools.Options"},
            new CommandEvent {CommandId = "Edit.Undo"},
            new CommandEvent {CommandId = "Edit.LineEndExtend"},
            new CommandEvent {CommandId = "View.ShowSmartTag"},
            new CommandEvent {CommandId = "View.ObjectBrowser"},
            new CommandEvent {CommandId = "View.SolutionExplorer"},
            new CommandEvent {CommandId = "View.TfsTeamExplorer"},
            new CommandEvent {CommandId = "View.ClassView"},
            new CommandEvent {CommandId = "View.Output"},
            new CommandEvent {CommandId = "Edit.Copy"},
            new CommandEvent {CommandId = "Edit.LineStart"},
            new CommandEvent {CommandId = "File.Exit"},
            new CommandEvent {CommandId = "Edit.WordPrevious"},
            new CommandEvent {CommandId = "Edit.WordNext"}
        };

        public class CommandCalculatorTestImplementation : CommandCalculator
        {
            public CommandCalculatorTestImplementation(IStatisticListing statisticListing,
                IMessageBus messageBus,
                IErrorHandler errorHandler) : base(statisticListing, messageBus, errorHandler) {}

            protected override IDEEvent FilterEvent(IDEEvent @event)
            {
                return @event;
            }
        }

        [Test, TestCaseSource("CommandEvents")]
        public void AddsNewCommandEventsCorrectly(CommandEvent commandEvent)
        {
            var actualStatistic = (CommandStatistic) ListingMock.Object.GetStatistic(StatisticType);

            Publish(commandEvent);

            Assert.AreEqual(1, actualStatistic.CommandTypeValues[commandEvent.CommandId]);
        }

        [Test]
        public void CalculateExistingCommandEvent()
        {
            var actualStatistic = (CommandStatistic) ListingMock.Object.GetStatistic(StatisticType);
            var commandEvent = new CommandEvent {CommandId = "Rename"};

            Publish(commandEvent);
            Publish(commandEvent);

            Assert.AreEqual(2, actualStatistic.CommandTypeValues[commandEvent.CommandId]);
        }

        [Test]
        public void EventCallWithWrongEventTypeExceptionHandlingTest()
        {
            Publish(new StatisticCalculatorTest.TestEvent());

            ListingMock.Verify(l => l.Update(It.IsAny<IStatistic>()), Times.Never);
        }
    }
}
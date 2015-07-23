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
using KaVE.Commons.Model.Events;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.VS.Achievements.Statistics.Calculators.BaseClasses;
using KaVE.VS.Achievements.Statistics.Filters;
using KaVE.VS.Achievements.Statistics.Listing;
using KaVE.VS.Achievements.Statistics.Statistics;
using KaVE.VS.Achievements.Tests.TestUtils;
using KaVE.VS.Achievements.UI.StatisticUI;
using KaVE.VS.Achievements.Util;
using KaVE.VS.FeedbackGenerator.MessageBus;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.Achievements.Tests.Statistics.Calculators
{
    [TestFixture]
    internal class StatisticCalculatorTest : CalculatorTest
    {
        public StatisticCalculatorTest() : base(typeof (TestCalculator), typeof (TestStatistic), new TestIDEEvent()) {}

        protected override bool IsNewStatistic(IStatistic statistic)
        {
            return statistic is TestStatistic;
        }

        internal class TestCalculator : StatisticCalculator
        {
            public TestCalculator(IStatisticListing statisticListing, IMessageBus messageBus, IErrorHandler errorHandler)
                :
                    base(statisticListing, messageBus, errorHandler, typeof (TestStatistic), new TestFilter()) {}

            protected override IStatistic Process(IDEEvent @event)
            {
                return new TestStatistic();
            }
        }

        /// <summary>
        ///     Uses a fault statistic in its constructor (constructing causes exception)
        /// </summary>
        internal class TestCalculatorWithBadStatisticType : StatisticCalculator
        {
            public TestCalculatorWithBadStatisticType(IStatisticListing statisticListing,
                IMessageBus messageBus,
                IErrorHandler errorHandler) :
                    base(
                    statisticListing,
                    messageBus,
                    errorHandler,
                    typeof (TestStatisticWithoutZeroArgsConstructor),
                    new TestFilter()) {}

            protected override IStatistic Process(IDEEvent @event)
            {
                return null;
            }

            public Type GetStatisticType()
            {
                return StatisticType;
            }
        }

        internal class TestFilter : IEventFilter
        {
            public IDEEvent Process(IDEEvent @event)
            {
                return @event;
            }
        }

        /// <summary>
        ///     Basic Implementation of IStatistic
        /// </summary>
        internal class TestStatisticWithoutZeroArgsConstructor : IStatistic
        {
            /// <summary>
            ///     To test exception handling for initialize statistic
            ///     (needs a constructor with 0 arguments)
            /// </summary>
            public TestStatisticWithoutZeroArgsConstructor(int i) {}

            public List<StatisticElement> GetCollection()
            {
                return new List<StatisticElement>();
            }
        }

        [Test]
        public void IgnoreNullEventsTest()
        {
            var testCalculator = new TestCalculator(ListingMock.Object, MessageBus.Object, ErrorHandlerMock.Object);

            testCalculator.Event(null);

            ListingMock.Verify(l => l.Update(It.IsAny<IStatistic>()), Times.Never);
        }

        [Test]
        public void InitializeStatisticExceptionHandlingTest()
        {
            var uut = new TestCalculatorWithBadStatisticType(
                ListingMock.Object,
                MessageBus.Object,
                ErrorHandlerMock.Object);

            var typeString = uut.GetStatisticType().Name;
            ErrorHandlerMock.Verify(
                handler =>
                    handler.SendErrorMessageToLogger(
                        It.IsAny<Exception>(),
                        string.Format(
                            "Statistic of type {0} must implement a constructor without parameters",
                            typeString)));
        }

        [Test]
        public void BlockAllEventsWhenInitializationWasWrong()
        {
            var uut = new TestCalculatorWithBadStatisticType(
                ListingMock.Object,
                MessageBus.Object,
                ErrorHandlerMock.Object);

            uut.Event(new TestIDEEvent());

            ListingMock.Verify(l => l.Update(It.IsAny<IStatistic>()), Times.Never);
        }
    }
}
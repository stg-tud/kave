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
using KaVE.VS.FeedbackGenerator.MessageBus;
using KaVE.VS.Statistics.Calculators.BaseClasses;
using KaVE.VS.Statistics.StatisticListing;
using KaVE.VS.Statistics.Statistics;
using KaVE.VS.Statistics.Tests.TestUtils;
using KaVE.VS.Statistics.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.Statistics.Tests.Calculators
{
    public abstract class CalculatorTest
    {
        protected Type CalculatorType;
        protected Mock<IErrorHandler> ErrorHandlerMock;
        protected IDEEvent EventForUpdateTest;
        protected Mock<IStatisticListing> ListingMock;
        protected Mock<IMessageBus> MessageBus;
        protected StatisticCalculator StatisticCalculator;
        protected Type StatisticType;

        protected CalculatorTest(Type calculatorType, Type statisticType, IDEEvent eventForUpdateTest)
        {
            CalculatorType = calculatorType;
            StatisticType = statisticType;
            EventForUpdateTest = eventForUpdateTest;
        }

        [SetUp]
        public void Init()
        {
            MessageBus = new Mock<IMessageBus>();
            ErrorHandlerMock = new Mock<IErrorHandler>();
            ListingMock = new Mock<IStatisticListing>();

            var initializedStatistic = (IStatistic) Activator.CreateInstance(StatisticType);
            ListingMock.Setup(l => l.GetStatistic(StatisticType)).Returns(initializedStatistic);

            var args = new object[] {ListingMock.Object, MessageBus.Object, ErrorHandlerMock.Object};
            StatisticCalculator = (StatisticCalculator) Activator.CreateInstance(CalculatorType, args);
        }

        [Test]
        public void SubscribeTest()
        {
            var messageBus = new TestMessageBus();

            Activator.CreateInstance(CalculatorType, ListingMock.Object, messageBus, ErrorHandlerMock.Object);

            messageBus.Publish(EventForUpdateTest);

            ListingMock.Verify(l => l.Update(It.IsAny<IStatistic>()), Times.Once);
        }

        [Test]
        public void UpdatesListingOnEventTest()
        {
            ListingMock.Object.GetStatistic(StatisticType);

            Publish(EventForUpdateTest);

            ListingMock.Verify(l => l.Update(It.Is<IStatistic>(statistic => statistic.GetType() == StatisticType)));
        }

        [Test]
        public void InitializeStatisticTest()
        {
            StatisticCalculator.InitializeStatistic();

            ListingMock.Verify(l => l.Update(It.Is<IStatistic>(statistic => IsNewStatistic(statistic))));
        }

        [Test]
        public void InitializeTestOnGetStatisticsReturnNullTest()
        {
            ListingMock = new Mock<IStatisticListing>();
            ListingMock.Setup(l => l.GetStatistic(StatisticType)).Returns((IStatistic) null);

            var args = new object[] {ListingMock.Object, MessageBus.Object, ErrorHandlerMock.Object};
            StatisticCalculator = (StatisticCalculator) Activator.CreateInstance(CalculatorType, args);

            ListingMock.Verify(l => l.Update(It.Is<IStatistic>(statistic => IsNewStatistic(statistic))));
        }

        protected abstract bool IsNewStatistic(IStatistic statistic);

        protected void Publish(IEnumerable<IDEEvent> events)
        {
            foreach (var @event in events)
            {
                StatisticCalculator.Event(@event);
            }
        }

        protected void Publish(IDEEvent @event)
        {
            StatisticCalculator.Event(@event);
        }
    }
}
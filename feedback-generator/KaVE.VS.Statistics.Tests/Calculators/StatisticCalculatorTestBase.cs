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
using KaVE.Commons.Utils.Exceptions;
using KaVE.VS.FeedbackGenerator.MessageBus;
using KaVE.VS.Statistics.Calculators.BaseClasses;
using KaVE.VS.Statistics.Statistics;
using KaVE.VS.Statistics.Tests.TestUtils;
using KaVE.VS.Statistics.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.Statistics.Tests.Calculators
{
    public abstract class StatisticCalculatorTestBase<TCalculator, TStatistic>
        where TCalculator : IStatisticCalculator
        where TStatistic : IStatistic, new()
    {
        protected Mock<ILogger> ErrorHandlerMock;
        protected Mock<IStatisticListing> ListingMock;
        protected Mock<IMessageBus> MessageBus;
        protected IDEEvent EventForUpdateTest;
        protected TCalculator Sut;

        protected StatisticCalculatorTestBase(IDEEvent eventForUpdateTest)
        {
            EventForUpdateTest = eventForUpdateTest;
        }

        [SetUp]
        public void Init()
        {
            MessageBus = new Mock<IMessageBus>();
            ErrorHandlerMock = new Mock<ILogger>();
            ListingMock = new Mock<IStatisticListing>();

            var args = new object[] {ListingMock.Object, MessageBus.Object, ErrorHandlerMock.Object};
            Sut = (TCalculator) Activator.CreateInstance(typeof (TCalculator), args);

            ListingMock.Setup(l => l.GetStatistic<TStatistic>()).Returns(new TStatistic());
            ListingMock.ResetCalls();
        }

        [Test]
        public void SubscribeTest()
        {
            var messageBus = new TestMessageBus();
            Activator.CreateInstance(typeof (TCalculator), ListingMock.Object, messageBus, ErrorHandlerMock.Object);
            messageBus.Publish(EventForUpdateTest);
            ListingMock.Verify(l => l.Update(It.IsAny<IStatistic>()), Times.Once);
        }

        [Test]
        public void UpdatesListingOnEventTest()
        {
            ListingMock.Object.GetStatistic<TStatistic>();
            Publish(EventForUpdateTest);
            ListingMock.Verify(l => l.Update(It.Is<IStatistic>(statistic => statistic.GetType() == Sut.StatisticType)));
        }

        [Test]
        public void InitializeStatisticTest()
        {
            Sut.InitializeStatistic();

            ListingMock.Verify(l => l.Update(It.Is<IStatistic>(statistic => IsNewStatistic(statistic))));
        }

        [Test]
        public void ShouldIgnoreFilteredEvents()
        {
            Sut.Event(null);
            ListingMock.Verify(l => l.Update(It.IsAny<IStatistic>()), Times.Never);
        }

        [Test]
        public void InitializeTestOnGetStatisticsReturnNullTest()
        {
            ListingMock = new Mock<IStatisticListing>();
            ListingMock.Setup(l => l.GetStatistic<TStatistic>()).Returns(default(TStatistic));

            var args = new object[] {ListingMock.Object, MessageBus.Object, ErrorHandlerMock.Object};
            Sut = (TCalculator) Activator.CreateInstance(typeof (TCalculator), args);

            ListingMock.Verify(l => l.Update(It.Is<IStatistic>(statistic => IsNewStatistic(statistic))));
        }

        protected abstract bool IsNewStatistic(IStatistic statistic);

        protected void Publish(params IDEEvent[] events)
        {
            foreach (var @event in events)
            {
                Sut.Event(@event);
            }
        }

        protected TStatistic GetStatistic()
        {
            return ListingMock.Object.GetStatistic<TStatistic>();
        }
    }
}
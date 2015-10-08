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
using System.Collections.ObjectModel;
using System.IO;
using KaVE.Commons.Utils.IO;
using KaVE.VS.Statistics.Statistics;
using KaVE.VS.Statistics.Tests.TestUtils;
using KaVE.VS.Statistics.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.Statistics.Tests
{
    [TestFixture]
    public class StatisticListingTest
    {
        [SetUp]
        public void Init()
        {
            _ioUtilMock = new Mock<IIoUtils>();
            Registry.RegisterComponent(_ioUtilMock.Object);
            Registry.RegisterComponent(new Mock<IErrorHandler>().Object);
            _statisticListing = new StatisticListing();
            _statistic = new TestStatistic();
        }

        [TearDown]
        public void Reset()
        {
            Registry.Clear();
        }

        private Mock<IIoUtils> _ioUtilMock;

        private StatisticListing _statisticListing;

        private readonly Mock<IObserver<IStatistic>> _observer = new Mock<IObserver<IStatistic>>();

        private IStatistic _statistic;

        private readonly Dictionary<Type, IStatistic> _testDictionary = new Dictionary<Type, IStatistic>
        {
            {typeof (TestStatistic), new TestStatistic()}
        };

        private string SerializedTestDictionary
        {
            get { return JsonSerialization.JsonSerializeObject(_testDictionary); }
        }


        [Test]
        public void AddNewObjectToDictionaryAndGetStatisticSuccess()
        {
            _statisticListing.Update(_statistic);
            var actual = _statisticListing.GetStatistic(_statistic.GetType());

            Assert.AreEqual(_statistic, actual);
        }

        [Test]
        public void CreateFileOnFirstUpdateWhenFileDoesNotExist()
        {
            _statisticListing.Update(new TestStatistic());

            _ioUtilMock.Verify(x => x.CreateFile(It.IsAny<string>()));
        }

        [Test]
        public void DeleteDataTest()
        {
            var commandStatistic = new CommandStatistic();
            var completionStatistic = new CompletionStatistic();

            _statisticListing.Update(commandStatistic);
            _statisticListing.Update(completionStatistic);

            _statisticListing.DeleteData();

            var actualCommandStatistic = _statisticListing.GetStatistic(commandStatistic.GetType());

            var actualCompletionStatistic = _statisticListing.GetStatistic(completionStatistic.GetType());

            Assert.AreEqual(null, actualCommandStatistic);

            Assert.AreEqual(null, actualCompletionStatistic);

            _ioUtilMock.Verify(io => io.DeleteFile(It.IsAny<string>()));
        }

        [Test]
        public void DoesnotSendUpdateToSubscribersWhenIsSendingUpdateToObserversIsFalse()
        {
            var statisticMock = new Mock<IStatistic>();

            _statisticListing.BlockUpdateToObservers = true;

            _statisticListing.Update(statisticMock.Object);

            _observer.Verify(obs => obs.OnNext(statisticMock.Object), Times.Never());
        }

        [Test]
        public void FileIsRecreatedOnExceptionWhileInitializing()
        {
            SetFileExists(true);
            _ioUtilMock.Setup(x => x.ReadFile(It.IsAny<string>())).Throws<Exception>();

            _statisticListing = new StatisticListing();

            _ioUtilMock.Verify(x => x.DeleteFile(It.IsAny<string>()));
            _ioUtilMock.Verify(x => x.CreateFile(It.IsAny<string>()));
        }

        [Test]
        public void GetStatisticFail()
        {
            Assert.AreEqual(null, _statisticListing.GetStatistic(typeof (Collection<>)));
        }

        [Test]
        public void ObserverSubscribeAndNotifyTest()
        {
            _statisticListing.Subscribe(_observer.Object);

            _statisticListing.Update(_statistic);

            _observer.Verify(obs => obs.OnNext(_statistic), Times.Once());
        }

        [Test]
        public void ObserverUnsubscribeWhileUpdatingTest()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new ObserverTestImplementation(_statisticListing);

            _statisticListing.Subscribe(_observer.Object);

            _statisticListing.Update(_statistic);

            _observer.Verify(obs => obs.OnNext(_statistic), Times.Once());
        }

        [Test]
        public void ObserverUnsubscriberTest()
        {
            var unsubscriber = _statisticListing.Subscribe(_observer.Object);

            unsubscriber.Dispose();

            _statisticListing.Update(_statistic);

            _observer.Verify(obs => obs.OnNext(_statistic), Times.Never());
        }

        [Test]
        public void ReadStatisticsFromFileOnInitalize()
        {
            SetFileExists(true);
            _ioUtilMock.Setup(x => x.ReadFile(It.IsAny<string>())).Returns(SerializedTestDictionary);

            _statisticListing = new StatisticListing();

            CollectionAssert.AreEquivalent(_testDictionary, _statisticListing.StatisticDictionary);
        }

        [Test]
        public void SendUpdateToObserversWithAllStatisticsTest()
        {
            var commandStatistic = new CommandStatistic();
            var completionStatistic = new CompletionStatistic();

            _statisticListing.Update(commandStatistic);
            _statisticListing.Update(completionStatistic);

            _statisticListing.Subscribe(_observer.Object);

            _statisticListing.SendUpdateToObserversWithAllStatistics();

            _observer.Verify(obs => obs.OnNext(commandStatistic));
            _observer.Verify(obs => obs.OnNext(completionStatistic));
        }

        [Test]
        public void UpdateCreatesFileWhenFileDoesNotExist()
        {
            SetFileExists(false);

            _statisticListing.Update(_statistic);

            _ioUtilMock.Verify(x => x.CreateFile(It.IsAny<string>()));
        }

        [Test]
        public void UpdateWriteStatisticsToFileTest()
        {
            _ioUtilMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);

            _statisticListing.Update(_statistic);

            _ioUtilMock.Verify(x => x.OpenFile(It.IsAny<string>(), FileMode.Create, FileAccess.Write));
        }

        private void SetFileExists(bool fileExists)
        {
            _ioUtilMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(fileExists);
        }

        public class ObserverTestImplementation : IObserver<IStatistic>
        {
            private readonly IDisposable _unsubscriber;

            public ObserverTestImplementation(IObservable<IStatistic> observable)
            {
                _unsubscriber = observable.Subscribe(this);
            }

            public void OnNext(IStatistic value)
            {
                _unsubscriber.Dispose();
            }

            public void OnError(Exception error)
            {
                // nothing
            }

            public void OnCompleted()
            {
                // nothing
            }
        }
    }
}
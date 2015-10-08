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
using System.Linq;
using KaVE.VS.Statistics.Statistics;
using KaVE.VS.Statistics.Tests.TestUtils;
using KaVE.VS.Statistics.UI;
using KaVE.VS.Statistics.UI.Utils;
using KaVE.VS.Statistics.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.Statistics.Tests.UI
{
    [TestFixture]
    public class StatisticViewModelTest
    {
        [SetUp]
        public void Init()
        {
            StatisticViewModel.CollectionsChanged += delegate { _eventRaised = true; };

            _eventRaised = false;

            InitializeCommandStatistic();

            _statisticDictionary = new Dictionary<Type, IStatistic>
            {
                {typeof (BuildStatistic), new BuildStatistic()},
                {typeof (CommandStatistic), _commandStatistic},
                {typeof (CompletionStatistic), new CompletionStatistic()},
                {typeof (GlobalStatistic), new GlobalStatistic()},
                {typeof (SolutionStatistic), new SolutionStatistic()}
            };

            _listingMock = new Mock<IStatisticListing>();
            _listingMock.Setup(listing => listing.StatisticDictionary).Returns(_statisticDictionary);

            _observableMock = new Mock<IObservable<IStatistic>>();
            _uiDelegatorMock = new Mock<IStatisticsUiDelegator>();
            _errorHandlerMock = new Mock<IErrorHandler>();

            _uut = new StatisticViewModel(
                _listingMock.Object,
                _observableMock.Object,
                _uiDelegatorMock.Object,
                _errorHandlerMock.Object);
        }

        private StatisticViewModel _uut;

        private Mock<IStatisticListing> _listingMock;

        private Mock<IObservable<IStatistic>> _observableMock;

        private Mock<IStatisticsUiDelegator> _uiDelegatorMock;

        private Mock<IErrorHandler> _errorHandlerMock;

        private Dictionary<Type, IStatistic> _statisticDictionary;

        private CommandStatistic _commandStatistic;

        private bool _eventRaised;

        private static readonly object[][] Statistics =
        {
            new object[]
            {
                typeof (BuildStatistic),
                new BuildStatistic {FailedBuilds = 10, SuccessfulBuilds = 2}
            },
            new object[]
            {
                typeof (CompletionStatistic),
                new CompletionStatistic
                {
                    SavedKeystrokes = 12,
                    TotalCancelled = 15,
                    TotalCompleted = 10,
                    TotalProposals = 5
                }
            },
            new object[]
            {
                typeof (SolutionStatistic),
                new SolutionStatistic {ProjectItemsAdded = 15, SolutionsClosed = 5}
            },
            new object[]
            {
                typeof (GlobalStatistic),
                new GlobalStatistic {TotalNumberOfEdits = 10, CurrentNumberOfEditsBetweenCommits = 5}
            }
        };

        [TestCaseSource("Statistics")]
        public void UpdateCollectionsCorrectly(Type statisticType, IStatistic statistic)
        {
            _uut.OnNext(statistic);

            _statisticDictionary[statisticType] = statistic;

            _uiDelegatorMock.Verify(uiDeleg => uiDeleg.DelegateToStatisticUi(It.IsAny<Action>()));

            _uut.RefreshElementsForStatistic(statistic, _uut.CollectionDictionary[statisticType]);

            var expectedCollectionList = _statisticDictionary.Values;
            var actualCollectionList = _uut.CollectionDictionary.Values;

            AssertStatisticElementCollectionsAreEqual(expectedCollectionList, actualCollectionList);

            Assert.True(_eventRaised);
        }

        private static void AssertStatisticElementCollectionsAreEqual(IEnumerable<IStatistic> expected,
            IEnumerable<ObservableCollection<StatisticElement>> actual)
        {
            var expectedStatisticList = expected.ToList();
            var actualCollectionList = actual.ToList();

            for (var i = 0; i < expectedStatisticList.Count; i++)
            {
                var expectedStatisticCollection = expectedStatisticList[i].GetCollection();
                var actualStatisticCollection = actualCollectionList[i];

                CompareStatisticCollections(expectedStatisticCollection, actualStatisticCollection);
            }
        }

        private static void CompareStatisticCollections(IList<StatisticElement> expectedStatisticCollection,
            IList<StatisticElement> actualCollectionList)
        {
            for (var i = 0; i < expectedStatisticCollection.Count(); i++)
            {
                Assert.AreEqual(expectedStatisticCollection[i], actualCollectionList[i]);
            }
        }

        private void InitializeCommandStatistic()
        {
            _commandStatistic = new CommandStatistic();
            _commandStatistic.CommandTypeValues.Add("ShowOptions", 10);
            _commandStatistic.CommandTypeValues.Add("{66BD4C1D-3401-4BCC-A942-E4990827E6F7}:8289:", 1000);
            _commandStatistic.CommandTypeValues.Add("{5EFC7975-14BC-11CF-9B2B-00AA00573819}:26:Edit.Paste", 10000000);
            _commandStatistic.CommandTypeValues.Add("TextControl.Backspace", 1);
        }

        [Test]
        public void InitializeCollectionsCorrectly()
        {
            var expectedCollectionList = _statisticDictionary.Values;
            var actualCollectionList = _uut.CollectionDictionary.Values;

            AssertStatisticElementCollectionsAreEqual(expectedCollectionList, actualCollectionList);

            Assert.True(_eventRaised);
        }

        [Test]
        public void ResetCollectionsTest()
        {
            _uut.ResetCollections();
            InitializeCollectionsCorrectly();
        }

        [Test]
        public void SendErrorMessageWhenNoCollectionForStatisticOnUpdate()
        {
            _uut.OnNext(new TestStatistic());

            _errorHandlerMock.Verify(
                errorHandler =>
                    errorHandler.SendErrorMessageToLogger(It.IsAny<KeyNotFoundException>(), It.IsAny<string>()));

            _uiDelegatorMock.Verify(uiDeleg => uiDeleg.DelegateToStatisticUi(It.IsAny<Action>()), Times.Never);
        }

        [Test]
        public void SubscribeOnInitialize()
        {
            _observableMock.Verify(obs => obs.Subscribe(_uut));
        }

        [Test]
        public void UpdatesCommandStatisticCollectionCorrectly()
        {
            _commandStatistic.CommandTypeValues.Add("Open File", 1);
            _commandStatistic.CommandTypeValues.Add("Debug.Start", 1);
            _commandStatistic.CommandTypeValues.Add("Debug.Stop", 1);
            _commandStatistic.CommandTypeValues.Add("TextControl.Down", 1);
            _commandStatistic.CommandTypeValues["ShowOptions"] = 11;

            _uut.OnNext(_commandStatistic);

            _statisticDictionary[typeof (CommandStatistic)] = _commandStatistic;

            _uiDelegatorMock.Verify(uiDeleg => uiDeleg.DelegateToStatisticUi(It.IsAny<Action>()));

            _uut.RefreshElementsForStatistic(_commandStatistic, _uut.CollectionDictionary[typeof (CommandStatistic)]);

            var expectedCollectionList = _statisticDictionary.Values;
            var actualCollectionList = _uut.CollectionDictionary.Values;

            AssertStatisticElementCollectionsAreEqual(expectedCollectionList, actualCollectionList);

            Assert.True(_eventRaised);
        }
    }
}
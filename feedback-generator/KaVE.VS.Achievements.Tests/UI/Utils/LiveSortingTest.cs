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
using System.Linq;
using System.Resources;
using KaVE.Commons.Utils;
using KaVE.VS.Achievements.BaseClasses.AchievementTypes;
using KaVE.VS.Achievements.Properties;
using KaVE.VS.Achievements.UI.AchievementGrids;
using KaVE.VS.Achievements.UI.MainWindow;
using KaVE.VS.Achievements.UI.Utils;
using KaVE.VS.Achievements.Utils;
using KaVE.VS.Statistics.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.Achievements.Tests.UI.Utils
{
    [TestFixture, RequiresSTA]
    internal class LiveSortingTest
    {
        [SetUp]
        public void SetUp()
        {
            _clockMock = new Mock<IDateUtils>();
            var clockMock = _clockMock;
            Registry.RegisterComponent(new Mock<ITargetValueProvider>().Object);
            Registry.RegisterComponent(clockMock.Object);
            clockMock.Setup(c => c.Now).Returns(new DateTime(2015, 2, 25));

            _firstAchievement = new ProgressAchievement(1)
            {
                CompletionDateTime = new DateTime(2015, 2, 25),
                CurrentProgress = 1
            };
            _secondAchievement = new ProgressAchievement(2)
            {
                CompletionDateTime = new DateTime(2015, 2, 25),
                CurrentProgress = 3
            };
            _thirdAchievement = new ProgressAchievement(3)
            {
                CompletionDateTime = new DateTime(2015, 2, 25),
                CurrentProgress = 5
            };
            _fourthAchievement = new ProgressAchievement(4)
            {
                CompletionDateTime = new DateTime(2015, 2, 25),
                CurrentProgress = 7
            };
            _lastAchievement = new ProgressAchievement(5)
            {
                CompletionDateTime = new DateTime(2015, 2, 25),
                CurrentProgress = 8
            };

            _firstGrid = new ProgressAchievementGrid(_firstAchievement, Rm);
            _secondGrid = new ProgressAchievementGrid(_secondAchievement, Rm);
            _thirdGrid = new ProgressAchievementGrid(_thirdAchievement, Rm);
            _fourthGrid = new ProgressAchievementGrid(_fourthAchievement, Rm);
            _lastGrid = new ProgressAchievementGrid(_lastAchievement, Rm);


            _achievements.Add(1, _firstAchievement);
            _achievements.Add(2, _secondAchievement);
            _achievements.Add(3, _thirdAchievement);
            _achievements.Add(4, _fourthAchievement);
            _achievements.Add(5, _lastAchievement);


            _listingMock.Setup(l => l.GetAchievementDictionary()).Returns(_achievements);
            _control = new AchievementWindowControl(_listingMock.Object, new AchievementsUiDelegator());
        }

        [TearDown]
        public void TearDown()
        {
            Registry.Clear();
            _achievements.Clear();
        }

        private Mock<IDateUtils> _clockMock;

        private AchievementWindowControl _control;

        private static readonly ResourceManager Rm = AchievementText.ResourceManager;

        private static ProgressAchievement _firstAchievement;

        private static ProgressAchievement _secondAchievement;

        private static ProgressAchievement _thirdAchievement;

        private static ProgressAchievement _fourthAchievement;

        private static ProgressAchievement _lastAchievement;

        private static ProgressAchievementGrid _firstGrid;
        private static ProgressAchievementGrid _secondGrid;
        private static ProgressAchievementGrid _thirdGrid;
        private static ProgressAchievementGrid _fourthGrid;
        private static ProgressAchievementGrid _lastGrid;

        private readonly Mock<IAchievementListing> _listingMock = new Mock<IAchievementListing>();
        private readonly Dictionary<int, BaseAchievement> _achievements = new Dictionary<int, BaseAchievement>();

        private static void AssertCollectionsAreEqual(IList<AchievementGrid> expectedCollection,
            IList<AchievementGrid> actualCollection)
        {
            Assert.AreEqual(expectedCollection.Count, actualCollection.Count);

            for (var i = 0; i < expectedCollection.Count; i++)
            {
                Assert.AreEqual(expectedCollection[i].TitleString, actualCollection[i].TitleString);
            }
        }

        private List<AchievementGrid> GetAllPanelAchievements()
        {
            return _control.Panels[0].Children.Cast<AchievementGrid>().ToList();
        }


        [Test]
        public void InitialSortedTest()
        {
            var expectedCollection = new List<AchievementGrid>
            {
                _lastGrid,
                _fourthGrid,
                _thirdGrid,
                _secondGrid,
                _firstGrid
            };
            var actualCollection = GetAllPanelAchievements();

            AssertCollectionsAreEqual(expectedCollection, actualCollection);
        }

        [Test]
        public void LiveTest()
        {
            var expectedCollection = new List<AchievementGrid>
            {
                _secondGrid,
                _lastGrid,
                _fourthGrid,
                _thirdGrid,
                _firstGrid
            };
            _secondAchievement.CurrentProgress = 40;

            var actualCollection = GetAllPanelAchievements();

            AssertCollectionsAreEqual(expectedCollection, actualCollection);
        }

        [Test]
        public void LiveTest2()
        {
            var expectedCollection = new List<AchievementGrid>
            {
                _secondGrid,
                _firstGrid,
                _fourthGrid,
                _lastGrid,
                _thirdGrid
            };
            _secondAchievement.CurrentProgress = 40;
            _fourthAchievement.CurrentProgress = 30;
            _firstAchievement.CurrentProgress = 35;


            var actualCollection = GetAllPanelAchievements();

            AssertCollectionsAreEqual(expectedCollection, actualCollection);
        }

        [Test]
        public void LiveTest3()
        {
            var expectedCollection = new List<AchievementGrid>
            {
                _lastGrid,
                _thirdGrid,
                _secondGrid,
                _fourthGrid,
                _firstGrid
            };
            _secondAchievement.CurrentProgress = 40;
            _lastAchievement.CurrentProgress = 100;
            _thirdAchievement.CurrentProgress = 99;

            var actualCollection = GetAllPanelAchievements();

            AssertCollectionsAreEqual(expectedCollection, actualCollection);
        }
    }
}
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
using System.Windows.Controls;
using KaVE.VS.Achievements.UI.AchievementGrids;
using KaVE.VS.Achievements.UI.Utils;
using NUnit.Framework;

namespace KaVE.VS.Achievements.Tests.UI.Utils
{
    [TestFixture, RequiresSTA]
    public class SortableStackPanelTest
    {
        [SetUp]
        public void SetUp()
        {
            _uut = new SortableStackPanel();
            _uut.Children.Add(FirstAchievement);
            _uut.Children.Add(SecondAchievement);
            _uut.Children.Add(ThirdAchievement);
            _uut.Children.Add(FourthAchievement);
            _uut.Children.Add(LastAchievement);
        }

        [TearDown]
        public void TearDown()
        {
            _uut.Children.Clear();
        }

        private SortableStackPanel _uut;

        private static readonly AchievementGrid FirstAchievement = new AchievementGrid
        {
            CompletionDate = new DateTime(2015, 2, 25),
            TitleString = "First Achievement",
            Progression = new ProgressBar {Value = 1}
        };

        private static readonly AchievementGrid SecondAchievement = new AchievementGrid
        {
            CompletionDate = new DateTime(),
            TitleString = "Second Achievement",
            Progression = new ProgressBar {Value = 0.5}
        };

        private static readonly AchievementGrid ThirdAchievement = new AchievementGrid
        {
            CompletionDate = new DateTime(2015, 3, 25),
            TitleString = "Third Achievement",
            Progression = new ProgressBar {Value = 1}
        };

        private static readonly AchievementGrid FourthAchievement = new AchievementGrid
        {
            CompletionDate = new DateTime(),
            TitleString = "Fourth Achievement",
            Progression = new ProgressBar {Value = 0}
        };

        private static readonly AchievementGrid LastAchievement = new AchievementGrid
        {
            CompletionDate = new DateTime(),
            TitleString = "Last Achievement",
            Progression = new ProgressBar {Value = 0.33}
        };

        private static void AssertCollectionsAreEqual(IList<AchievementGrid> expectedCollection,
            IList<AchievementGrid> actualCollection)
        {
            Assert.AreEqual(expectedCollection.Count, actualCollection.Count);

            for (var i = 0; i < expectedCollection.Count; i++)
            {
                Assert.AreEqual(expectedCollection[i].TitleString, actualCollection[i].TitleString);
            }
        }

        [Test]
        public void DontSortWithWrongCriteria()
        {
            var beforeSortCall = _uut.Children.Cast<AchievementGrid>().ToList();

            _uut.SortBy("WrongCriteria");

            AssertCollectionsAreEqual(beforeSortCall, _uut.Children.Cast<AchievementGrid>().ToList());
        }

        [Test]
        public void SortByCompletionDateTest()
        {
            var expectedCollection = new List<AchievementGrid>
            {
                ThirdAchievement,
                FirstAchievement,
                SecondAchievement,
                FourthAchievement,
                LastAchievement
            };

            _uut.SortBy("CompletionDate");

            var actualCollection = _uut.Children.Cast<AchievementGrid>().ToList();

            AssertCollectionsAreEqual(expectedCollection, actualCollection);
        }

        [Test]
        public void SortByProgressionTest()
        {
            var expectedCollection = new List<AchievementGrid>
            {
                FirstAchievement,
                ThirdAchievement,
                SecondAchievement,
                LastAchievement,
                FourthAchievement
            };

            _uut.SortBy("Progression");

            var actualCollection = _uut.Children.Cast<AchievementGrid>().ToList();

            AssertCollectionsAreEqual(expectedCollection, actualCollection);
        }

        [Test]
        public void SortByTitleTest()
        {
            var expectedCollection = new List<AchievementGrid>
            {
                FirstAchievement,
                FourthAchievement,
                LastAchievement,
                SecondAchievement,
                ThirdAchievement
            };

            _uut.SortBy("Title");

            var actualCollection = _uut.Children.Cast<AchievementGrid>().ToList();

            AssertCollectionsAreEqual(expectedCollection, actualCollection);
        }
    }
}
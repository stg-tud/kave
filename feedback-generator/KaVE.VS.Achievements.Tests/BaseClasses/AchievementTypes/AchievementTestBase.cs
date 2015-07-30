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
using KaVE.Commons.Utils;
using KaVE.VS.Achievements.Utils;
using KaVE.VS.Statistics.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.Achievements.Tests.BaseClasses.AchievementTypes
{
    public abstract class AchievementTestBase
    {
        public static DateTime TestTime = DateTime.Now;

        protected static Mock<ITargetValueProvider> TargetValueProviderMock;
        protected Mock<IDateUtils> ClockMock;

        protected DateTime Now
        {
            get { return ClockMock.Object.Now; }
        }

        [SetUp]
        public void SetUp()
        {
            ClockMock = new Mock<IDateUtils>();
            Registry.RegisterComponent(ClockMock.Object);
            ClockMock.Setup(c => c.Now).Returns(TestTime);

            TargetValueProviderMock = new Mock<ITargetValueProvider>();
            Registry.RegisterComponent(TargetValueProviderMock.Object);
            TargetValueProviderMock.Setup(tvp => tvp.GetTargetValue(It.IsAny<int>())).Returns(int.MaxValue);
        }

        [TearDown]
        public void ClearRegistry()
        {
            Registry.Clear();
        }

        protected void ReturnTargetValueForId(object value, int id)
        {
            TargetValueProviderMock.Setup(tvp => tvp.GetTargetValue(It.Is<int>(i => i == id))).Returns(value);
        }

        protected void SetTimeTo(DateTime dateTime)
        {
            ClockMock.Setup(c => c.Now).Returns(dateTime);
        }
    }
}
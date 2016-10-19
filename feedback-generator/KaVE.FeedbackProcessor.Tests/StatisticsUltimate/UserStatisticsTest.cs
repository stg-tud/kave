﻿/*
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
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.FeedbackProcessor.StatisticsUltimate;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.StatisticsUltimate
{
    internal class UserStatisticsTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new UserStatistics();
            Assert.AreEqual(DateTime.MinValue, sut.DayFirst);
            Assert.AreEqual(DateTime.MinValue, sut.DayLast);
            Assert.AreEqual(0, sut.NumDays);
            Assert.AreEqual(0, sut.NumMonth);
            Assert.AreEqual(0, sut.NumEvents);
            Assert.AreEqual(Educations.Unknown, sut.Education);
            Assert.AreEqual(Positions.Unknown, sut.Position);
            Assert.AreEqual(0, sut.NumCodeCompletion);
            Assert.AreEqual(0, sut.NumTestRuns);

            Assert.AreNotEqual(0, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var now = DateTime.Now;
            var sut = new UserStatistics
            {
                DayFirst = now,
                DayLast = now,
                NumDays = 1,
                NumMonth = 2,
                NumEvents = 3,
                Education = Educations.Bachelor,
                Position = Positions.Student,
                NumCodeCompletion = 4,
                NumTestRuns = 5
            };
            Assert.AreEqual(now, sut.DayFirst);
            Assert.AreEqual(now, sut.DayLast);
            Assert.AreEqual(1, sut.NumDays);
            Assert.AreEqual(2, sut.NumMonth);
            Assert.AreEqual(3, sut.NumEvents);
            Assert.AreEqual(Educations.Bachelor, sut.Education);
            Assert.AreEqual(Positions.Student, sut.Position);
            Assert.AreEqual(4, sut.NumCodeCompletion);
            Assert.AreEqual(5, sut.NumTestRuns);
        }

        [Test]
        public void EqualityDefault()
        {
            var a = new UserStatistics();
            var b = new UserStatistics();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void ReallyEqual()
        {
            var now = DateTime.Now;
            var a = new UserStatistics
            {
                DayFirst = now,
                DayLast = now,
                NumDays = 1,
                NumMonth = 2,
                NumEvents = 3,
                Education = Educations.Bachelor,
                Position = Positions.Student,
                NumCodeCompletion = 4,
                NumTestRuns = 5
            };
            var b = new UserStatistics
            {
                DayFirst = now,
                DayLast = now,
                NumDays = 1,
                NumMonth = 2,
                NumEvents = 3,
                Education = Educations.Bachelor,
                Position = Positions.Student,
                NumCodeCompletion = 4,
                NumTestRuns = 5
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Different_DayFirst()
        {
            var now = DateTime.Now;
            var a = new UserStatistics();
            var b = new UserStatistics
            {
                DayFirst = now
            };
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Different_DayLast()
        {
            var now = DateTime.Now;
            var a = new UserStatistics();
            var b = new UserStatistics
            {
                DayLast = now
            };
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Different_NumDays()
        {
            var a = new UserStatistics();
            var b = new UserStatistics
            {
                NumDays = 1
            };
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Different_NumMonth()
        {
            var a = new UserStatistics();
            var b = new UserStatistics
            {
                NumMonth = 2
            };
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Different_NumEvents()
        {
            var a = new UserStatistics();
            var b = new UserStatistics
            {
                NumEvents = 3
            };
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Different_Education()
        {
            var a = new UserStatistics();
            var b = new UserStatistics
            {
                Education = Educations.Bachelor
            };
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Different_Position()
        {
            var a = new UserStatistics();
            var b = new UserStatistics
            {
                Position = Positions.Student
            };
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Different_NumCodeCompletion()
        {
            var a = new UserStatistics();
            var b = new UserStatistics
            {
                NumCodeCompletion = 4
            };
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Different_NumTestRuns()
        {
            var a = new UserStatistics();
            var b = new UserStatistics
            {
                NumTestRuns = 5
            };
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}
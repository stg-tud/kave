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
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.WatchdogExports.Model
{
    internal class IntervalTest
    {
        [Test]
        public void EndTimeIsCorrect()
        {
            var i = new TestInterval {StartTime = DateTime.Today, Duration = TimeSpan.FromMinutes(1)};

            Assert.AreEqual(DateTime.Today.AddMinutes(1), i.EndTime);
        }

        [Test]
        public void Equality_Default()
        {
            var i1 = new TestInterval();
            var i2 = new TestInterval();

            Assert.AreEqual(i1, i2);
        }

        [Test]
        public void Equality_StartTime()
        {
            var i1 = new TestInterval {StartTime = DateTime.Today};
            var i2 = new TestInterval {StartTime = DateTime.Today};
            var i3 = new TestInterval {StartTime = DateTime.Today.AddMinutes(1)};

            Assert.AreEqual(i1, i2);
            Assert.AreNotEqual(i2, i3);
        }

        [Test]
        public void Equality_Duration()
        {
            var i1 = new TestInterval {Duration = TimeSpan.FromMinutes(1)};
            var i2 = new TestInterval {Duration = TimeSpan.FromMinutes(1)};
            var i3 = new TestInterval {Duration = TimeSpan.FromMinutes(2)};

            Assert.AreEqual(i1, i2);
            Assert.AreNotEqual(i2, i3);
        }

        [Test]
        public void Equality_UserId()
        {
            var i1 = new TestInterval {UserId = "a"};
            var i2 = new TestInterval {UserId = "a"};
            var i3 = new TestInterval {UserId = "b"};

            Assert.AreEqual(i1, i2);
            Assert.AreNotEqual(i2, i3);
        }

        [Test]
        public void Equality_KaVEVersion()
        {
            var i1 = new TestInterval {KaVEVersion = "a"};
            var i2 = new TestInterval {KaVEVersion = "a"};
            var i3 = new TestInterval {KaVEVersion = "b"};

            Assert.AreEqual(i1, i2);
            Assert.AreNotEqual(i2, i3);
        }

        [Test]
        public void Equality_IDESessionId()
        {
            var i1 = new TestInterval {IDESessionId = "a"};
            var i2 = new TestInterval {IDESessionId = "a"};
            var i3 = new TestInterval {IDESessionId = "b"};

            Assert.AreEqual(i1, i2);
            Assert.AreNotEqual(i2, i3);
        }

        [Test]
        public void Equality_Project()
        {
            var i1 = new TestInterval {Project = "a"};
            var i2 = new TestInterval {Project = "a"};
            var i3 = new TestInterval {Project = "b"};

            Assert.AreEqual(i1, i2);
            Assert.AreNotEqual(i2, i3);
        }
    }
}
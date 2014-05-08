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
using KaVE.Model.Events;
using NUnit.Framework;

namespace KaVE.Model.Tests.Events
{
    [TestFixture]
    class IDEEventTest
    {
        [Test]
        public void ShouldDeriveDurationFromStartAndEndTime()
        {
            var expected = TimeSpan.FromSeconds(1);
            var now = DateTime.Now;
            var infoEvent = new InfoEvent
            {
                TriggeredAt = now,
                TerminatedAt = now.AddSeconds(1)
            };

            Assert.AreEqual(expected, infoEvent.Duration);
        }

        [Test]
        public void ShouldDeriveEndTimeFromStartTimeAndDuration()
        {
            var now = DateTime.Now;
            var expected = now.AddSeconds(3);
            var infoEvent = new InfoEvent
            {
                TriggeredAt = now,
                Duration = TimeSpan.FromSeconds(3)
            };

            Assert.AreEqual(expected, infoEvent.TerminatedAt);
        }

        [Test]
        public void ShouldHaveNoEndTimeWithoutDuration()
        {
            var infoEvent = new InfoEvent
            {
                TriggeredAt = DateTime.Now,
                Duration = null
            };

            Assert.IsNull(infoEvent.TerminatedAt);
        }

        [Test]
        public void ShouldHaveNoEndTimeWithoutStartTime()
        {
            var infoEvent = new InfoEvent
            {
                TriggeredAt = null,
                Duration = TimeSpan.FromMinutes(9)
            };

            Assert.IsNull(infoEvent.TerminatedAt);
        }
    }
}

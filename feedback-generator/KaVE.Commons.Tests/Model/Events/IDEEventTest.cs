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
using KaVE.Commons.Model.Naming;
using KaVE.Commons.TestUtils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Events
{
    internal class IDEEventTest
    {
        #region default values and equality

        [Test]
        public void DefaultValues()
        {
            var sut = new TestIDEEvent();
            Assert.AreEqual(null, sut.Id);
            Assert.AreEqual(null, sut.ActiveDocument);
            Assert.AreEqual(null, sut.ActiveWindow);
            Assert.AreEqual(null, sut.Duration);
            Assert.AreEqual(null, sut.IDESessionUUID);
            Assert.AreEqual(null, sut.KaVEVersion);
            Assert.AreEqual(null, sut.TerminatedAt);
            Assert.AreEqual(null, sut.TriggeredAt);
            Assert.AreEqual(EventTrigger.Unknown, sut.TriggeredBy);
        }

        [Test]
        public void SettingValues()
        {
            var sut = new TestIDEEvent
            {
                Id = "1",
                ActiveDocument = Names.Document("d d"),
                ActiveWindow = Names.Window("w w"),
                // Duration is automatically set
                IDESessionUUID = "2",
                KaVEVersion = "3",
                TriggeredAt = DateTime.Today.AddDays(-1),
                TerminatedAt = DateTime.Today,
                TriggeredBy = EventTrigger.Click
            };
            Assert.AreEqual("1", sut.Id);
            Assert.AreEqual(Names.Document("d d"), sut.ActiveDocument);
            Assert.AreEqual(Names.Window("w w"), sut.ActiveWindow);
            Assert.AreEqual(TimeSpan.FromDays(1), sut.Duration);
            Assert.AreEqual("2", sut.IDESessionUUID);
            Assert.AreEqual("3", sut.KaVEVersion);
            Assert.AreEqual(DateTime.Today, sut.TerminatedAt);
            Assert.AreEqual(DateTime.Today.AddDays(-1), sut.TriggeredAt);
            Assert.AreEqual(EventTrigger.Click, sut.TriggeredBy);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new TestIDEEvent();
            var b = new TestIDEEvent();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new TestIDEEvent
            {
                Id = "1",
                ActiveDocument = Names.Document("d d"),
                ActiveWindow = Names.Window("w w"),
                // Duration is automatically set
                IDESessionUUID = "2",
                KaVEVersion = "3",
                TriggeredAt = DateTime.Today.AddDays(-1),
                TerminatedAt = DateTime.Today,
                TriggeredBy = EventTrigger.Click
            };
            var b = new TestIDEEvent
            {
                Id = "1",
                ActiveDocument = Names.Document("d d"),
                ActiveWindow = Names.Window("w w"),
                // Duration is automatically set
                IDESessionUUID = "2",
                KaVEVersion = "3",
                TriggeredAt = DateTime.Today.AddDays(-1),
                TerminatedAt = DateTime.Today,
                TriggeredBy = EventTrigger.Click
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentId()
        {
            var a = new TestIDEEvent
            {
                Id = "1"
            };
            var b = new TestIDEEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new TestIDEEvent());
        }

        // TODO implement remaining equality tests

        #endregion

        [Test]
        public void ShouldDeriveDurationFromStartAndEndTime()
        {
            var expected = TimeSpan.FromSeconds(1);
            var now = DateTime.Now;
            var ideEvent = new TestIDEEvent
            {
                TriggeredAt = now,
                TerminatedAt = now.AddSeconds(1)
            };

            Assert.AreEqual(expected, ideEvent.Duration);
        }

        [Test]
        public void ShouldDeriveEndTimeFromStartTimeAndDuration()
        {
            var now = DateTime.Now;
            var expected = now.AddSeconds(3);
            var ideEvent = new TestIDEEvent
            {
                TriggeredAt = now,
                Duration = TimeSpan.FromSeconds(3)
            };

            Assert.AreEqual(expected, ideEvent.TerminatedAt);
        }

        [Test]
        public void ShouldHaveNoEndTimeWithoutDuration()
        {
            var ideEvent = new TestIDEEvent
            {
                TriggeredAt = DateTime.Now,
                Duration = null
            };

            Assert.IsNull(ideEvent.TerminatedAt);
        }

        [Test]
        public void ShouldHaveNoEndTimeWithoutStartTime()
        {
            var ideEvent = new TestIDEEvent
            {
                TriggeredAt = null,
                Duration = TimeSpan.FromMinutes(9)
            };

            Assert.IsNull(ideEvent.TerminatedAt);
        }

        private class TestIDEEvent : IDEEvent {}
    }
}
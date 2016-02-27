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

using KaVE.Commons.Model.Events;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Events
{
    internal class SystemEventTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new SystemEvent();
            Assert.AreEqual(SystemEventType.Unknown, sut.Type);
        }

        [Test]
        public void SettingValues()
        {
            var sut = new SystemEvent {Type = SystemEventType.Resume};
            Assert.AreEqual(SystemEventType.Resume, sut.Type);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new SystemEvent();
            var b = new SystemEvent();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new SystemEvent {Type = SystemEventType.Resume};
            var b = new SystemEvent {Type = SystemEventType.Resume};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentBaseClassInfo()
        {
            var a = new SystemEvent {IDESessionUUID = "a"};
            var b = new SystemEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentType()
        {
            var a = new SystemEvent {Type = SystemEventType.Resume};
            var b = new SystemEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void NumberingOfEnumIsStable()
        {
            Assert.AreEqual(0, (int) SystemEventType.Unknown);
            Assert.AreEqual(1, (int) SystemEventType.Suspend);
            Assert.AreEqual(2, (int) SystemEventType.Resume);
            Assert.AreEqual(3, (int) SystemEventType.Lock);
            Assert.AreEqual(4, (int) SystemEventType.Unlock);
            Assert.AreEqual(5, (int) SystemEventType.RemoteConnect);
            Assert.AreEqual(6, (int) SystemEventType.RemoteDisconnect);
        }
    }
}
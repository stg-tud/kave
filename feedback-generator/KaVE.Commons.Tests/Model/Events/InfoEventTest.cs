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
    internal class InfoEventTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new InfoEvent();
            Assert.Null(sut.Info);
        }

        [Test]
        public void SetValues()
        {
            var sut = new InfoEvent {Info = "A"};
            Assert.AreEqual("A", sut.Info);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new InfoEvent();
            var b = new InfoEvent();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new InfoEvent {Info = "A"};
            var b = new InfoEvent {Info = "A"};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_BaseIsCalled()
        {
            var a = new InfoEvent {IDESessionUUID = "a"};
            var b = new InfoEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentInfo()
        {
            var a = new InfoEvent {Info = "a"};
            var b = new InfoEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}
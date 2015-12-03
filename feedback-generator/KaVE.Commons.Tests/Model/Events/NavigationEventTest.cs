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
using KaVE.Commons.Model.Events.VersionControlEvents;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.TestUtils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Events
{
    internal class NavigationEventTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new NavigationEvent();
            Assert.AreEqual(Name.UnknownName, sut.Target);
            Assert.AreEqual(Name.UnknownName, sut.Location);
        }

        [Test]
        public void SetValues_Target()
        {
            var sut = new NavigationEvent {Target = Name.Get("A")};
            Assert.AreEqual(Name.Get("A"), sut.Target);
        }

        [Test]
        public void SetValues_Location()
        {
            var sut = new NavigationEvent {Location = Name.Get("A")};
            Assert.AreEqual(Name.Get("A"), sut.Location);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new NavigationEvent();
            var b = new NavigationEvent();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_BaseClassIsCalled()
        {
            var a = new NavigationEvent {IDESessionUUID = "a"};
            var b = new NavigationEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new NavigationEvent {Target = Name.Get("A"), Location = Name.Get("B")};
            var b = new NavigationEvent {Target = Name.Get("A"), Location = Name.Get("B")};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentTarget()
        {
            var a = new NavigationEvent {Target = Name.Get("A"), Location = Name.Get("B")};
            var b = new NavigationEvent {Target = Name.Get("C"), Location = Name.Get("B")};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentLocation()
        {
            var a = new NavigationEvent {Target = Name.Get("A"), Location = Name.Get("B")};
            var b = new NavigationEvent {Target = Name.Get("A"), Location = Name.Get("C")};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new VersionControlEvent());
        }
    }
}
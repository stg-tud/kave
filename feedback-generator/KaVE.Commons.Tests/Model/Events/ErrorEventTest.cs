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
    internal class ErrorEventTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new ErrorEvent();
            Assert.Null(sut.Content);
            Assert.Null(sut.StackTrace);
        }

        [Test]
        public void SetValues_Content()
        {
            var sut = new ErrorEvent {Content = "A"};
            Assert.AreEqual("A", sut.Content);
            Assert.Null(sut.StackTrace);
        }

        [Test]
        public void SetValues_StackTrace()
        {
            var sut = new ErrorEvent {StackTrace = new[] {"X"}};
            Assert.Null(sut.Content);
            Assert.AreEqual(new[] {"X"}, sut.StackTrace);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new ErrorEvent();
            var b = new ErrorEvent();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_BaseClassIsCalled()
        {
            var a = new ErrorEvent {IDESessionUUID = "a"};
            var b = new ErrorEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new ErrorEvent {Content = "A", StackTrace = new[] {"X"}};
            var b = new ErrorEvent {Content = "A", StackTrace = new[] {"X"}};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentContent()
        {
            var a = new ErrorEvent {Content = "A"};
            var b = new ErrorEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentStacktrace()
        {
            var a = new ErrorEvent {StackTrace = new[] {"X"}};
            var b = new ErrorEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}
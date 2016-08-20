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
using KaVE.FeedbackProcessor.Preprocessing.Filters;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Preprocessing.Filters
{
    internal class VersionFilterTest
    {
        [Test]
        public void NameIsCorrect()
        {
            Assert.AreEqual("VersionFilter(>=3)", new VersionFilter(3).Name);
        }

        [Test]
        public void Null()
        {
            AssertFalse(null);
        }

        [Test]
        public void Empty()
        {
            AssertFalse("");
        }

        [Test]
        public void Smaller()
        {
            AssertFalse("0.2-default");
        }

        [Test]
        public void Equals()
        {
            AssertTrue("0.3-default");
        }

        [Test]
        public void Bigger()
        {
            AssertTrue("0.4-default");
        }

        [Test]
        public void QualifierIsCaseIndependent()
        {
            AssertTrue("0.4-DefAult");
        }

        [Test]
        public void OptionalQualifier()
        {
            AssertTrue("0.4");
        }

        [Test]
        public void NonDefault()
        {
            AssertFalse("0.4-feature");
        }

        [Test]
        public void UnparseableString()
        {
            AssertFalse("Some-Unknown-Version-Format");
        }

        [Test]
        public void UnparseableInteger()
        {
            AssertFalse("0.11111111111111111111111111111111111111111111-default");
        }

        private static void AssertFalse(string versionStr)
        {
            Assert.False(Eval(versionStr));
        }

        private static void AssertTrue(string versionStr)
        {
            Assert.True(Eval(versionStr));
        }

        private static bool Eval(string versionStr)
        {
            var sut = new VersionFilter(3);
            var @event = new CommandEvent {KaVEVersion = versionStr};
            return sut.Func(@event);
        }
    }
}
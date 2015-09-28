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
using KaVE.Commons.Model.Events.VersionControlEvents;
using KaVE.Commons.TestUtils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Events.VersionControlEvents
{
    internal class VersionControlActionTest
    {
        private static readonly DateTime SomeDateTime = new DateTime(903492580);
        private const VersionControlActionType SomeGitAction = VersionControlActionType.Commit;

        [Test]
        public void DefaultValues()
        {
            var uut = new VersionControlAction();
            Assert.AreEqual(new DateTime(), uut.ExecutedAt);
            Assert.AreEqual(VersionControlActionType.Unknown, uut.ActionType);
        }

        [Test]
        public void SettingValues()
        {
            var actualEvent = new VersionControlAction
            {
                ExecutedAt = SomeDateTime,
                ActionType = SomeGitAction
            };

            Assert.AreEqual(SomeDateTime, actualEvent.ExecutedAt);
            Assert.AreEqual(SomeGitAction, actualEvent.ActionType);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new VersionControlAction();
            var b = new VersionControlAction();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new VersionControlAction {ExecutedAt = SomeDateTime, ActionType = SomeGitAction};
            var b = new VersionControlAction {ExecutedAt = SomeDateTime, ActionType = SomeGitAction};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentExecutedAt()
        {
            var a = new VersionControlAction {ExecutedAt = SomeDateTime, ActionType = SomeGitAction};
            var b = new VersionControlAction {ActionType = SomeGitAction};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentActionType()
        {
            var a = new VersionControlAction {ExecutedAt = SomeDateTime, ActionType = SomeGitAction};
            var b = new VersionControlAction {ExecutedAt = SomeDateTime};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void ToStringTest()
        {
            ToStringAssert.Reflection(new VersionControlAction());
        }
    }
}
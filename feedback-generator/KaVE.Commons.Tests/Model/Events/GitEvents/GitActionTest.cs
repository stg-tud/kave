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
using KaVE.Commons.Model.Events.GitEvents;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Events.GitEvents
{
    internal class GitActionTest
    {
        private static readonly DateTime SomeDateTime = new DateTime(903492580);
        private const GitActionType SomeGitAction = GitActionType.Commit;

        [Test]
        public void DefaultValues()
        {
            Assert.AreEqual(null, new GitAction().ExecutedAt);
            Assert.AreEqual(GitActionType.Unknown, new GitAction().ActionType);
        }

        [Test]
        public void SettingValues()
        {
            var actualEvent = new GitAction
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
            var a = new GitAction();
            var b = new GitAction();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new GitAction {ExecutedAt = SomeDateTime, ActionType = SomeGitAction};
            var b = new GitAction {ExecutedAt = SomeDateTime, ActionType = SomeGitAction};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentExecutedAt()
        {
            var a = new GitAction {ExecutedAt = SomeDateTime, ActionType = SomeGitAction};
            var b = new GitAction {ActionType = SomeGitAction};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentActionType()
        {
            var a = new GitAction { ExecutedAt = SomeDateTime, ActionType = SomeGitAction };
            var b = new GitAction { ExecutedAt = SomeDateTime };
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void ToStringTest()
        {
            var action = new GitAction {ExecutedAt = new DateTime(1924, 8, 23, 19, 48, 12), ActionType = GitActionType.Commit};

            Assert.AreEqual("[23.08.1924 19:48:12] Commit", action.ToString());
        }
    }
}
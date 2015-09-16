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

using KaVE.Commons.Model.Events.VersionControlEvents;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Events.VersionControlEvents
{
    internal class VersionControlActionTypeTest
    {
        [Test]
        public void ShouldParseNonSpecialCase()
        {
            Assert.AreEqual(VersionControlActionType.Commit, "commit".ToVersionControlActionType());
        }

        [Test]
        public void ShouldParseCommitAmend()
        {
            Assert.AreEqual(VersionControlActionType.CommitAmend, "commit (amend)".ToVersionControlActionType());
        }

        [Test]
        public void ShouldParsePull()
        {
            Assert.AreEqual(
                VersionControlActionType.Pull,
                "pull --progress origin +refs/heads/master:refs/heads/master".ToVersionControlActionType());
        }

        [Test]
        public void ShouldParseRebaseFinished()
        {
            Assert.AreEqual(VersionControlActionType.RebaseFinished, "rebase finished".ToVersionControlActionType());
        }

        [Test]
        public void ShouldParseMerge()
        {
            Assert.AreEqual(VersionControlActionType.Merge, "merge origin/master".ToVersionControlActionType());
        }

        [Test]
        public void ShouldParseCommitInitial()
        {
            Assert.AreEqual(VersionControlActionType.CommitInitial, "commit (initial)".ToVersionControlActionType());
        }
    }
}
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

using KaVE.Commons.Model.Events.GitEvents;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Events.GitEvents
{
    internal class GitActionTypeTest
    {
        [Test]
        public void ShouldParseNonSpecialCase()
        {
            Assert.AreEqual(GitActionType.Commit, "commit".ToGitActionType());
        }

        [Test]
        public void ShouldParseCommitAmend()
        {
            Assert.AreEqual(GitActionType.CommitAmend, "commit (amend)".ToGitActionType());
        }

        [Test]
        public void ShouldParsePull()
        {
            Assert.AreEqual(
                GitActionType.Pull,
                "pull --progress origin +refs/heads/master:refs/heads/master".ToGitActionType());
        }

        [Test]
        public void ShouldParseRebaseFinished()
        {
            Assert.AreEqual(GitActionType.RebaseFinished, "rebase finished".ToGitActionType());
        }

        [Test]
        public void ShouldParseMerge()
        {
            Assert.AreEqual(GitActionType.Merge, "merge origin/master".ToGitActionType());
        }
    }
}
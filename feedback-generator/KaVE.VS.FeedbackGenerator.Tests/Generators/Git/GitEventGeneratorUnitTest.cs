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
using System.Collections.Generic;
using KaVE.Commons.Model.Events.VersionControlEvents;
using KaVE.Commons.Model.Names.VisualStudio;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;
using KaVE.VS.FeedbackGenerator.Generators.Git;
using KaVE.VS.FeedbackGenerator.MessageBus;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators.Git
{
    internal class GitEventGeneratorUnitTest : EventGeneratorTestBase
    {
        private static IEnumerable<string> InitialContent
        {
            get
            {
                return Lists.NewList(
                    @"0000000000000000000000000000000000000000 0f3314871233b43aabed70b544b06d7bebfc7bd5 TestUsername <TestMail@domain.de> 1431617414 +0200	clone: from https://github.com/repo.git",
                    @"0f3314871233b43aabed70b544b06d7bebfc7bd5 21dffc94c537ccde68bbf11b929aeb90f800260b TestUsername <TestMail@domain.de> 1431617566 +0200	commit: commit message",
                    @"21dffc94c537ccde68bbf11b929aeb90f800260b bba5acda9a7c3fcb32db4bd89bc7710e769429ab TestUsername <TestMail@domain.de> 1431619728 +0200	pull --progress origin +refs/heads/master:refs/remotes/origin/master: Fast-forward");
            }
        }

        private static IEnumerable<VersionControlAction> InitialContentActions
        {
            get
            {
                return Lists.NewList(
                    new VersionControlAction
                    {
                        ActionType = VersionControlActionType.Clone,
                        ExecutedAt = new DateTime(2015, 05, 14, 17, 30, 14)
                    },
                    new VersionControlAction
                    {
                        ActionType = VersionControlActionType.Commit,
                        ExecutedAt = new DateTime(2015, 05, 14, 17, 32, 46)
                    },
                    new VersionControlAction
                    {
                        ActionType = VersionControlActionType.Pull,
                        ExecutedAt = new DateTime(2015, 05, 14, 18, 08, 48)
                    }
                    );
            }
        }

        private const string TestCommitString =
            "de75df3fd4322ec96e02c078e90228f121b6b53c 6f2eaaff6079e41af242a41a09b5f9510214d014 TestUsername <TestMail@domain.de> 1441217745 +0200	commit: Test commit";

        private static VersionControlAction TestCommitAction
        {
            get
            {
                return new VersionControlAction
                {
                    ActionType = VersionControlActionType.Commit,
                    ExecutedAt = new DateTime(2015, 9, 2, 20, 15, 45)
                };
            }
        }

        private static readonly SolutionName SomeSolution = SolutionName.Get("SomeSolution");

        private TestGitEventGenerator _uut;

        [SetUp]
        public void Setup()
        {
            _uut = new TestGitEventGenerator(TestRSEnv, TestMessageBus, TestDateUtils);

            foreach (var line in InitialContent)
            {
                WriteLine(line);
            }
        }

        [Test]
        public void ShouldWriteAllContentOnInitialEvent()
        {
            _uut.OnGitHistoryFileChanged(null, new GitLogFileChangedEventArgs(string.Empty, SomeSolution));

            var actualEvent = GetSinglePublished<VersionControlEvent>();
            CollectionAssert.AreEqual(InitialContentActions, actualEvent.Actions);
        }

        [Test]
        public void ShouldWriteContentDeltaOnFileChanged()
        {
            _uut.OnGitHistoryFileChanged(null, new GitLogFileChangedEventArgs(string.Empty, SomeSolution));

            WriteLine(TestCommitString);

            _uut.OnGitHistoryFileChanged(null, new GitLogFileChangedEventArgs(string.Empty, SomeSolution));

            var actualEvent = GetLastPublished<VersionControlEvent>();
            CollectionAssert.AreEqual(Lists.NewList(TestCommitAction), actualEvent.Actions);
        }

        [Test]
        public void ShouldSetSolutionOnFileChanged()
        {
            WriteLine(TestCommitString);
            _uut.OnGitHistoryFileChanged(null, new GitLogFileChangedEventArgs(string.Empty, SomeSolution));

            var actualEvent = GetSinglePublished<VersionControlEvent>();
            Assert.AreEqual(SomeSolution, actualEvent.Solution);
        }

        [Test]
        public void ShouldHandleUserNamesWithWhitespaces()
        {
            ClearLog();

            const string stringWithWhitespaceUserName =
                "de75df3fd4322ec96e02c078e90228f121b6b53c 6f2eaaff6079e41af242a41a09b5f9510214d014 Test Username <TestMail@domain.de> 1441217745 +0200	commit: Test commit";

            WriteLine(stringWithWhitespaceUserName);

            _uut.OnGitHistoryFileChanged(null, new GitLogFileChangedEventArgs(string.Empty, SomeSolution));

            var actualEvent = GetSinglePublished<VersionControlEvent>();
            CollectionAssert.AreEqual(Lists.NewList(TestCommitAction), actualEvent.Actions);
            Assert.AreEqual(SomeSolution, actualEvent.Solution);
        }

        [Test]
        public void ShouldNotFireWhenLogIsEmpty()
        {
            ClearLog();

            _uut.OnGitHistoryFileChanged(null, new GitLogFileChangedEventArgs(string.Empty, SomeSolution));

            AssertNoEvent();
        }

        [Test]
        public void ShouldNotAddActionWhenTimeStampCantBeExtracted()
        {
            ClearLog();

            const string stringWithInvalidTimeStamp =
                "de75df3fd4322ec96e02c078e90228f121b6b53c 6f2eaaff6079e41af242a41a09b5f9510214d014 TestUsername <TestMail@domain.de> invalidtimestamp +0200	commit: Test commit";

            WriteLine(stringWithInvalidTimeStamp);

            _uut.OnGitHistoryFileChanged(null, new GitLogFileChangedEventArgs(string.Empty, SomeSolution));

            AssertNoEvent();
        }

        [Test]
        public void ShouldNotAddActionWhenActionTypeCantBeExtracted()
        {
            ClearLog();

            const string stringWithInvalidTimeStamp =
                "de75df3fd4322ec96e02c078e90228f121b6b53c 6f2eaaff6079e41af242a41a09b5f9510214d014 TestUsername <TestMail@domain.de> 1441217745 +0200	jghntohj: Test commit";

            WriteLine(stringWithInvalidTimeStamp);

            _uut.OnGitHistoryFileChanged(null, new GitLogFileChangedEventArgs(string.Empty, SomeSolution));

            AssertNoEvent();
        }

        private void ClearLog()
        {
            _uut.Content.Clear();
        }

        private void WriteLine(string newLine)
        {
            _uut.Content.Add(newLine);
        }

        private class TestGitEventGenerator : GitEventGenerator
        {
            public readonly IKaVEList<string> Content;

            public TestGitEventGenerator([NotNull] IRSEnv env,
                [NotNull] IMessageBus messageBus,
                [NotNull] IDateUtils dateUtils)
                : base(env, messageBus, dateUtils)
            {
                Content = Lists.NewList<string>();
            }

            protected override IEnumerable<string> ReadLogContent(string fullPath)
            {
                return Content;
            }
        }
    }
}
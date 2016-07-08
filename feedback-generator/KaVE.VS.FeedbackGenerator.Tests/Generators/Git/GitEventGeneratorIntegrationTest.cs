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
using System.IO;
using System.Threading;
using JetBrains.ProjectModel;
using JetBrains.Util;
using KaVE.Commons.Model.Events.VersionControlEvents;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;
using KaVE.VS.FeedbackGenerator.Generators.Git;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators.Git
{
    internal class GitEventGeneratorIntegrationTest : EventGeneratorTestBase
    {
        private static readonly string[] InitialContent =
        {
            @"0000000000000000000000000000000000000000 0f3314871233b43aabed70b544b06d7bebfc7bd5 TestUsername <TestMail@domain.de> 1431617414 +0200	clone: from https://github.com/stg-tud/kave.git",
            @"0f3314871233b43aabed70b544b06d7bebfc7bd5 21dffc94c537ccde68bbf11b929aeb90f800260b TestUsername <TestMail@domain.de> 1431617566 +0200	commit: commit message",
            @"21dffc94c537ccde68bbf11b929aeb90f800260b bba5acda9a7c3fcb32db4bd89bc7710e769429ab TestUsername <TestMail@domain.de> 1431619728 +0200	pull --progress origin +refs/heads/master:refs/remotes/origin/master: Fast-forward"
        };

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

        private static VersionControlAction TestCommitGitAction
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

        private string _dirTmp;

        [NotNull]
        private string FileGitLog
        {
            get { return _dirTmp + @"\repo\.git\logs\HEAD"; }
        }

        private string FileSolution
        {
            get { return _dirTmp + @"\repo\project\SomeSolution.sln"; }
        }

        private GitEventGenerator _uut;

        private Mock<ISolution> _solutionMock;

        private GitLogFileChangedRegistration _watcher;

        [SetUp]
        public void Setup()
        {
            _dirTmp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_dirTmp);
            CreateParent(FileGitLog);
            CreateParent(FileSolution);

            WriteInitialContentToLogFile();

            _solutionMock = new Mock<ISolution>();
            _solutionMock.Setup(solution => solution.SolutionFilePath).Returns(FileSystemPath.Parse(FileSolution));
            _solutionMock.Setup(solution => solution.Name).Returns("SomeSolution");
            _uut = new GitEventGenerator(TestRSEnv, TestMessageBus, TestDateUtils);

            _watcher = new GitLogFileChangedRegistration(_solutionMock.Object, _uut);
        }

        [TearDown]
        public void DeleteTemporaryFilesAndFolders()
        {
            _watcher.Dispose();

            if (Directory.Exists(_dirTmp))
            {
                Directory.Delete(_dirTmp, true);
            }
        }

        [Test]
        public void ShouldFireEventWithCompleteContentAtInitialization()
        {
            var initialEvent = GetSinglePublished<VersionControlEvent>();
            Assert.AreEqual(Names.Solution("SomeSolution"), initialEvent.Solution);
            CollectionAssert.AreEqual(InitialContentActions, initialEvent.Actions);
        }

        [Test]
        public void ShouldFireEventWithDeltaAtLogFileChange()
        {
            WriteLine(TestCommitString);

            Thread.Sleep(500);

            var actualEvent = GetLastPublished<VersionControlEvent>();
            Assert.AreEqual(Names.Solution("SomeSolution"), actualEvent.Solution);
            CollectionAssert.AreEqual(Lists.NewList(TestCommitGitAction), actualEvent.Actions);
        }

        [Test]
        public void ShouldNotFireWhenLogIsEmpty()
        {
            File.WriteAllText(FileGitLog, string.Empty);

            Thread.Sleep(500);

            // assert only the initial event was published
            GetSinglePublished<VersionControlEvent>();
        }

        private static void CreateParent(string path)
        {
            var directoryInfo = Directory.GetParent(path);
            Directory.CreateDirectory(directoryInfo.FullName);
        }

        private void WriteInitialContentToLogFile()
        {
            using (var stream = new StreamWriter(FileGitLog))
            {
                foreach (var line in InitialContent)
                {
                    stream.WriteLine(line);
                }
            }
        }

        private void WriteLine(string content)
        {
            using (var stream = new StreamWriter(FileGitLog))
            {
                stream.WriteLine(content);
            }
        }
    }
}
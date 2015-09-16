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
using System.IO;
using System.Threading;
using JetBrains.ProjectModel;
using JetBrains.Util;
using KaVE.Commons.Model.Events.VersionControlEvents;
using KaVE.Commons.Model.Names.VisualStudio;
using KaVE.Commons.Utils.Collections;
using KaVE.VS.FeedbackGenerator.Generators.Git;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators.Git
{
    internal class GitEventGeneratorIntegrationTest : EventGeneratorTestBase
    {
        private const string TestCommitString =
            "de75df3fd4322ec96e02c078e90228f121b6b53c 6f2eaaff6079e41af242a41a09b5f9510214d014 TestUsername <TestMail@domain.de> 1441217745 +0200	commit: Test commit";

        private string _dirTmp;

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
        private GitHistoryFileChangedRegistration _watcher;

        [SetUp]
        public void Setup()
        {
            _dirTmp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_dirTmp);
            CreateParent(FileGitLog);
            CreateParent(FileSolution);

            _solutionMock = new Mock<ISolution>();
            _solutionMock.Setup(solution => solution.SolutionFilePath).Returns(FileSystemPath.Parse(FileSolution));
            _solutionMock.Setup(solution => solution.Name).Returns("SomeSolution");
            _uut = new GitEventGenerator(TestRSEnv, TestMessageBus, TestDateUtils);

            _watcher = new GitHistoryFileChangedRegistration(_solutionMock.Object, _uut);
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

        private static void CreateParent(string path)
        {
            var directoryInfo = Directory.GetParent(path);
            Directory.CreateDirectory(directoryInfo.FullName);
        }

        [Test]
        public void IntegrationTest()
        {
            using (var stream = new StreamWriter(FileGitLog))
            {
                stream.Write(TestCommitString);
            }

            Thread.Sleep(500);

            var actualEvent = GetSinglePublished<VersionControlEvent>();
            var expectedGitAction = new VersionControlAction
            {
                ActionType = VersionControlActionType.Commit,
                ExecutedAt = new DateTime(2015, 9, 2, 20, 15, 45)
            };
            Assert.AreEqual(SolutionName.Get("SomeSolution"), actualEvent.Solution);
            CollectionAssert.AreEqual(Lists.NewList(expectedGitAction), actualEvent.Content);
        }
    }
}
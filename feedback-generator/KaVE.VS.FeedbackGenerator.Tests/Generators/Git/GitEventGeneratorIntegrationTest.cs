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
using KaVE.Commons.Model.Events;
using KaVE.VS.FeedbackGenerator.Generators.Git;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators.Git
{
    [Ignore("Run these tests manually with your own TemporaryTestDirectory")]
    internal class GitEventGeneratorIntegrationTest : EventGeneratorTestBase
    {
        // WARNING: this directory will be created automatically but not deleted
        private const string TemporaryTestDirectory = @"H:\IntegrationTest\";

        private const string RelativeSolutionPath = @"feedback-generator\KaVE.Feedback.sln";
        private const string RelativeGitLogPath = @".git\logs";

        private const string TestRepositoryName = "kave";
        private const string GitLogFileName = "HEAD";

        private const string TestCommitString =
            "de75df3fd4322ec96e02c078e90228f121b6b53c 6f2eaaff6079e41af242a41a09b5f9510214d014 M8is <M8is@live.de> 1441217745 +0200	commit: Test commit";

        private static string TestRepositoryDirectory
        {
            get { return Path.Combine(TemporaryTestDirectory, TestRepositoryName); }
        }

        private static string GitLogPath
        {
            get { return Path.Combine(TestRepositoryDirectory, RelativeGitLogPath); }
        }

        private static string GitLogFile
        {
            get { return Path.Combine(GitLogPath, GitLogFileName); }
        }

        private static string SolutionFile
        {
            get { return Path.Combine(TestRepositoryDirectory, RelativeSolutionPath); }
        }

        private GitEventGenerator _uut;
        private Mock<ISolution> _solutionMock;

        [SetUp]
        public void Setup()
        {
            Directory.CreateDirectory(GitLogPath);

            _solutionMock = new Mock<ISolution>();
            _solutionMock.Setup(solution => solution.SolutionFilePath).Returns(FileSystemPath.Parse(SolutionFile));
            _uut = new GitEventGenerator(TestRSEnv, TestMessageBus, TestDateUtils);

            // ReSharper disable once UnusedVariable
            var registerSolution = new GitHistoryFileChangedRegistration(_solutionMock.Object, _uut);
        }

        [Test, Ignore("Manual test")]
        public void IntegrationTest()
        {
            using (var stream = new StreamWriter(GitLogFile))
            {
                stream.Write(TestCommitString);
            }

            Thread.Sleep(500);

            var actualEvent = GetSinglePublished<GitEvent>();
            Assert.AreEqual(TestCommitString, actualEvent.Content);
            Assert.AreEqual(TestRepositoryDirectory, actualEvent.RepositoryDirectory);
        }

        [TearDown]
        public void DeleteTemporaryFilesAndFolders()
        {
            File.Delete(GitLogFile);
            
            try
            {
                DeleteTemporaryDirectory();
            }
            catch (Exception exception)
            {
                MessageBox.ShowInfo("Deleting the temporary directory failed: " + exception.Message);
            }
        }

        private static void DeleteTemporaryDirectory()
        {
            // this is a workaround for avoiding a "The directory is not empty" exception
            // see http://zacharykniebel.com/blog/web-development/2013/june/21/solving-the-csharp-bug-when-recursively-deleting-directories
            try
            {
                Directory.Delete(TemporaryTestDirectory, true);
            }
            catch (IOException)
            {
                Directory.Delete(TemporaryTestDirectory, true);
            }
        }
    }
}
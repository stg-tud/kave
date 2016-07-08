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

using System.IO;
using System.Threading;
using JetBrains.ProjectModel;
using JetBrains.Util;
using KaVE.Commons.Model.Naming;
using KaVE.VS.FeedbackGenerator.Generators.Git;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators.Git
{
    internal class GitLogFileChangedRegistrationTest
    {
        private const string SomeSolution = "SomeSolution";

        private string _dirTmp;

        private string FileGitLog
        {
            get { return _dirTmp + @"\repo\.git\logs\HEAD"; }
        }

        private string DirectoryGit
        {
            get { return _dirTmp + @"\repo\.git\"; }
        }

        private string FileSolution
        {
            get { return _dirTmp + @"\repo\project\SomeSolution.sln"; }
        }

        private Mock<ISolution> _solutionMock;
        private EventGeneratorDummy _eventGeneratorDummy;
        private GitLogFileChangedRegistration _uut;

        [SetUp]
        public void Setup()
        {
            _dirTmp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_dirTmp);
            Directory.CreateDirectory(DirectoryGit);
            CreateParent(FileSolution);

            _solutionMock = new Mock<ISolution>();
            _solutionMock.Setup(solution => solution.SolutionFilePath).Returns(FileSystemPath.Parse(FileSolution));
            _solutionMock.Setup(solution => solution.Name).Returns(SomeSolution);

            _eventGeneratorDummy = new EventGeneratorDummy();

            _uut = new GitLogFileChangedRegistration(_solutionMock.Object, _eventGeneratorDummy);
        }

        [TearDown]
        public void DeleteTemporaryFilesAndFolders()
        {
            if (_uut != null)
            {
                _uut.Dispose();
            }

            if (Directory.Exists(_dirTmp))
            {
                Directory.Delete(_dirTmp, true);
            }
        }

        [Test]
        public void ShouldCreateGitLogFileOnConstruction()
        {
            Assert.IsTrue(File.Exists(FileGitLog));
        }

        [Test]
        public void ShouldGenerateCorrectEventArgsOnConstruction()
        {
            var actualArgs = _eventGeneratorDummy.OnGitHistoryFileChangedArgs;
            Assert.NotNull(actualArgs);
            Assert.AreEqual(FileGitLog, actualArgs.FullPath);
            Assert.AreEqual(Names.Solution(SomeSolution), actualArgs.Solution);
        }

        [Test]
        public void ShouldTriggerInitialEventOnConstruction()
        {
            Assert.IsTrue(_eventGeneratorDummy.EventFired);
        }

        [Test]
        public void ShouldTriggerEventOnFileChanged()
        {
            _eventGeneratorDummy.EventFired = false;

            WriteLine("Some line");
            Thread.Sleep(500);

            Assert.IsTrue(_eventGeneratorDummy.EventFired);
        }

        [Test]
        public void ShouldGenerateCorrectEventArgsOnFileChanged()
        {
            _eventGeneratorDummy.OnGitHistoryFileChangedArgs = null;
            WriteLine("Some line");
            Thread.Sleep(500);

            var actualArgs = _eventGeneratorDummy.OnGitHistoryFileChangedArgs;
            Assert.NotNull(actualArgs);
            Assert.AreEqual(FileGitLog, actualArgs.FullPath);
            Assert.AreEqual(Names.Solution(SomeSolution), actualArgs.Solution);
        }

        [Test]
        public void ShouldNotFailAtConstructionIfGitFolderDoesntExist()
        {
            _uut.Dispose();
            if (Directory.Exists(DirectoryGit))
            {
                Directory.Delete(DirectoryGit, true);
            }

            _uut = new GitLogFileChangedRegistration(_solutionMock.Object, _eventGeneratorDummy);
        }

        private static void CreateParent(string path)
        {
            var directoryInfo = Directory.GetParent(path);
            Directory.CreateDirectory(directoryInfo.FullName);
        }

        private void WriteLine(string content)
        {
            using (var stream = new StreamWriter(FileGitLog))
            {
                stream.WriteLine(content);
            }
        }

        public class EventGeneratorDummy : IGitEventGenerator
        {
            public bool EventFired;

            public GitLogFileChangedEventArgs OnGitHistoryFileChangedArgs;

            public void OnGitHistoryFileChanged(object sender, GitLogFileChangedEventArgs args)
            {
                EventFired = true;
                OnGitHistoryFileChangedArgs = args;
            }
        }
    }
}
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

using System.Collections.Generic;
using KaVE.Commons.Utils.Collections;
using KaVE.RS.SolutionAnalysis.SortByUser;
using Moq;
using NUnit.Framework;

namespace KaVE.RS.SolutionAnalysis.Tests.SortByUser
{
    internal class SortByUserRunnerTest
    {
        #region test setup and helpers

        private ISortByUserIo _io;
        private ISortByUserLogger _log;
        private SortByUserRunner _sut;

        private Dictionary<string, IKaVESet<string>> _ids;
        private List<IKaVESet<string>> _mergedFiles;

        [SetUp]
        public void Setup()
        {
            _ids = new Dictionary<string, IKaVESet<string>>();
            _mergedFiles = new List<IKaVESet<string>>();

            _io = Mock.Of<ISortByUserIo>();
            Mock.Get(_io).Setup(io => io.ScanArchivesForIdentifiers()).Returns(_ids);
            Mock.Get(_io)
                .Setup(io => io.MergeArchives(It.IsAny<IKaVESet<string>>()))
                .Callback<IKaVESet<string>>(files => _mergedFiles.Add(files));

            _log = Mock.Of<ISortByUserLogger>();
            _sut = new SortByUserRunner(_io, _log);
        }

        private void AddFile(string fileName, params string[] ids)
        {
            _ids[fileName] = Sets.NewHashSetFrom(ids);
        }

        private void AssertMerge(params string[] files)
        {
            CollectionAssert.Contains(_mergedFiles, Sets.NewHashSetFrom(files));
        }

        #endregion

        [Test]
        public void MergesFilesWithOverlap()
        {
            AddFile("1.zip", "a", "x");
            AddFile("2.zip", "a", "y");
            AddFile("3.zip", "b", "z");

            _sut.Run();

            AssertMerge("1.zip", "2.zip");
            AssertMerge("3.zip");
        }

        [Test]
        public void LoggerIsCalled()
        {
            AddFile("1.zip", "a", "x");
            AddFile("2.zip", "a", "y");
            AddFile("3.zip", "b", "z");

            _sut.Run();

            var u1 = new User
            {
                Files = {"1.zip", "2.zip"},
                Identifiers = {"a", "x", "y"}
            };
            var u2 = new User
            {
                Files = {"3.zip"},
                Identifiers = {"b", "z"}
            };

            Mock.Get(_log).Verify(log => log.StartScanning());
            Mock.Get(_log).Verify(log => log.FoundUsers(Sets.NewHashSet(u1, u2)));
            Mock.Get(_log).Verify(log => log.Reassembling());
            Mock.Get(_log).Verify(log => log.UserResult(u1));
            Mock.Get(_log).Verify(log => log.UserResult(u2));
        }
    }
}
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

using KaVE.Commons.Utils.Collections;
using KaVE.RS.SolutionAnalysis.SortByUser;
using NUnit.Framework;

namespace KaVE.RS.SolutionAnalysis.Tests.SortByUser
{
    internal class SortByUserLoggerTest
    {
        private SortByUserLogger _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new SortByUserLogger();
        }

        [Test]
        public void IntegrationTest()
        {
            _sut.StartScanning();
            ScanArchivesForIdentifiers();

            _sut.FoundUsers(CreateSomeUsers());

            _sut.StartMerging();
            foreach (var user in CreateSomeUsers())
            {
                _sut.UserResult(user);
                MergeArchives(user.Files);
            }
            _sut.FinalStats(100, 50);
        }

        private void ScanArchivesForIdentifiers()
        {
            foreach (var fileName in new[] {"1.zip", "2.zip"})
            {
                _sut.ReadingArchive(fileName);

                GetEventsFromArchive(123);
                for (var i = 0; i < 2*123; i++)
                {
                    _sut.CountInputEvent();
                }
            }
        }

        private void GetEventsFromArchive(int num)
        {
            for (var i = 0; i < num; i++)
            {
                _sut.Progress();
            }
        }

        private void MergeArchives(IKaVESet<string> files)
        {
            _sut.Merging(files);
            foreach (var fileName in files)
            {
                _sut.ReadingArchive(fileName);
                GetEventsFromArchive(107);
                _sut.StoreOutputEvents(107);
            }

            _sut.WritingArchive("xyz");
        }

        #region helpers

        private IKaVESet<User> CreateSomeUsers()
        {
            var users = Sets.NewHashSet<User>();
            users.Add(
                new User
                {
                    Identifiers = {"i1", "i2"},
                    Files = {"f1", "f2", "f3"}
                });
            users.Add(
                new User
                {
                    Identifiers = {"i3", "i4", "i5"},
                    Files = {"f4", "f5"}
                });
            return users;
        }

        #endregion
    }
}
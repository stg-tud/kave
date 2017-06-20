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
using System.IO;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Json;
using KaVE.FeedbackProcessor.Preprocessing;
using KaVE.FeedbackProcessor.Preprocessing.Logging;
using Moq;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Preprocessing
{
    internal class IdReaderTest : FileBasedPreprocessingTestBase
    {
        private IdReader _sut;
        private IIdReaderLogger _log;
        private IKaVEList<IEnumerable<string>> _foundIds;


        [SetUp]
        public void Setup()
        {
            _foundIds = Lists.NewList<IEnumerable<string>>();
            _log = Mock.Of<IIdReaderLogger>();
            Mock.Get(_log)
                .Setup(l => l.FoundIds(It.IsAny<IEnumerable<string>>()))
                .Callback<IEnumerable<string>>(s => _foundIds.Add(s));
            _sut = new IdReader(_log);
        }

        private void Given(params IDEEvent[] es)
        {
            WriteZip(Path.Combine(RawDir, "a.zip"), es);
        }

        private void AssertIds(params string[] expecteds)
        {
            var actuals = _sut.Read(Path.Combine(RawDir, "a.zip"));
            CollectionAssert.AreEquivalent(expecteds, actuals);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void ZipHasToExist()
        {
            _sut.Read("C:\\Does.NotExist.zip");
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void RejectsNonZips()
        {
            _sut.Read("C:\\Windows\\notepad.exe");
        }

        [Test]
        public void ReadingIdsWorks()
        {
            Given(
                new CommandEvent {IDESessionUUID = "s1"},
                new CommandEvent {IDESessionUUID = "s2"},
                new CommandEvent {IDESessionUUID = "s1"},
                new UserProfileEvent {ProfileId = "p1"});

            AssertIds("sid:s1", "sid:s2", "pid:p1");
        }

        [Test]
        public void NullAndEmptyIsNotUsedAsId()
        {
            Given(
                new CommandEvent {IDESessionUUID = " "},
                new CommandEvent {IDESessionUUID = ""},
                new CommandEvent {IDESessionUUID = null},
                new UserProfileEvent {ProfileId = " "},
                new UserProfileEvent {ProfileId = ""},
                // ReSharper disable once AssignNullToNotNullAttribute
                new UserProfileEvent {ProfileId = null});

            AssertIds();
        }

        [Test]
        public void FileIsCreated()
        {
            Given(new CommandEvent {IDESessionUUID = "s1"});
            var cacheFile = Path.Combine(RawDir, "a.ids");
            Assert.IsFalse(File.Exists(cacheFile));
            _sut.Read(Path.Combine(RawDir, "a.zip"));
            Assert.IsTrue(File.Exists(cacheFile));
            var json = File.ReadAllText(cacheFile);
            var actuals = json.ParseJsonTo<KaVEHashSet<string>>();
            var expecteds = Sets.NewHashSet("sid:s1");
            Assert.AreEqual(expecteds, actuals);
        }

        [Test]
        public void FileIsUsed()
        {
            Given(new CommandEvent {IDESessionUUID = "s1"});
            var cacheFile = Path.Combine(RawDir, "a.ids");
            File.WriteAllText(cacheFile, @"[""sid:s2""]");
            AssertIds("sid:s2");
        }

        [Test]
        public void LoggerTest()
        {
            ReadingIdsWorks();
            ReadingIdsWorks();

            var zip = Path.Combine(RawDir, "a.zip");

            Mock.Get(_log).Verify(l => l.Processing(zip), Times.Exactly(2));
            Mock.Get(_log).Verify(l => l.CacheHit(), Times.Exactly(1));
            Mock.Get(_log).Verify(l => l.CacheMiss(), Times.Exactly(1));
            Mock.Get(_log).Verify(l => l.FoundIds(It.IsAny<IKaVESet<string>>()), Times.Exactly(2));

            Assert.AreEqual(2, _foundIds.Count);
            CollectionAssert.AreEquivalent(Sets.NewHashSet("sid:s1", "sid:s2", "pid:p1"), _foundIds[0]);
            CollectionAssert.AreEquivalent(Sets.NewHashSet("sid:s1", "sid:s2", "pid:p1"), _foundIds[1]);
        }
    }
}
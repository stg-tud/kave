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
using System.Linq;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Preprocessing;
using KaVE.FeedbackProcessor.Preprocessing.Logging;
using KaVE.FeedbackProcessor.Preprocessing.Model;
using Moq;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Preprocessing
{
    internal class MultiThreadedPreprocessingTest
    {
        private const int NumWorker = 4;
        private const int NumGroups = 10;
        private const int NumZipsPerGroup = 100;

        private readonly object _lock = new object();
        private IPreprocessingIo _io;
        private IMultiThreadedPreprocessingLogger _log;

        MultiThreadedPreprocessing _sut;

        private IKaVESet<string> _actualProcessedZips;
        private IDictionary<string, IKaVESet<string>> _actualGroupedIds;
        private IList<IKaVESet<string>> _actualGroups;
        private IKaVEList<string> _actualCleanedZips;

        private Exception _lastLoggedException;

        [SetUp]
        public void Setup()
        {
            _actualProcessedZips = Sets.NewHashSet<string>();
            _actualGroupedIds = null;
            _actualGroups = Lists.NewList<IKaVESet<string>>();
            _actualCleanedZips = Lists.NewList<string>();

            _io = Mock.Of<IPreprocessingIo>();
            Mock.Get(_io).Setup(io => io.FindRelativeZipPaths()).Returns(GetExpectedZips());
            Mock.Get(_io)
                .Setup(io => io.GetFullPath_In(It.IsAny<string>()))
                .Returns<string>(relZip => "<dirIn>\\" + relZip);
            Mock.Get(_io)
                .Setup(io => io.GetFullPath_Merged(It.IsAny<string>()))
                .Returns<string>(relZip => "<dirMerged>\\" + relZip);
            Mock.Get(_io)
                .Setup(io => io.GetFullPath_Out(It.IsAny<string>()))
                .Returns<string>(relZip => "<dirOut>\\" + relZip);

            _log = Mock.Of<IMultiThreadedPreprocessingLogger>();
            Mock.Get(_log)
                .Setup(l => l.Error(It.IsAny<int>(), It.IsAny<Exception>()))
                .Callback<int, Exception>((taskId, ex) => _lastLoggedException = ex);

            _sut = new MultiThreadedPreprocessing(
                _io,
                _log,
                NumWorker,
                IdReaderFactory,
                CreateGrouper(),
                GroupMergerFactory,
                CleanerFactory);
        }

        private IKaVESet<string> GetExpectedZips()
        {
            var zips = Sets.NewHashSet<string>();
            for (var i = 0; i < NumGroups; i++)
            {
                for (var j = 0; j < NumZipsPerGroup; j++)
                {
                    zips.Add("zip_" + i + "_" + j);
                }
            }
            return zips;
        }

        private IDictionary<string, IKaVESet<string>> GetExpectedIds()
        {
            var d = new Dictionary<string, IKaVESet<string>>();

            foreach (var relZip in GetExpectedZips())
            {
                var irrelevantId = relZip + "_..."; // no overlap ever
                var overlappingId = relZip.Substring(0, 5);
                d[relZip] = Sets.NewHashSet(irrelevantId, overlappingId);
            }

            return d;
        }

        private IKaVESet<IKaVESet<string>> GetExpectedGroups()
        {
            var groups = Sets.NewHashSet<IKaVESet<string>>();
            foreach (var g in GetExpectedZips().GroupBy(zip => zip.Substring(0, 5)))
            {
                groups.Add(Sets.NewHashSetFrom(g));
            }
            return groups;
        }

        private IKaVESet<string> GetExpectedCleanedZips()
        {
            var zips = Sets.NewHashSet<string>();
            foreach (var g in GetExpectedZips().GroupBy(zip => zip.Substring(0, 5)))
            {
                zips.Add(g.First());
            }
            return zips;
        }

        private IIdReader IdReaderFactory(int taskId)
        {
            var m = Mock.Of<IIdReader>();
            Mock.Get(m).Setup(o => o.Read(It.IsAny<string>())).Returns<string>(
                zip =>
                {
                    lock (_lock)
                    {
                        _actualProcessedZips.Add(zip);
                    }
                    var ids = GetExpectedIds();
                    var relZip = zip.Substring(8);
                    return ids[relZip];
                });
            return m;
        }

        public IGrouper CreateGrouper()
        {
            var grouper = Mock.Of<IGrouper>();
            Mock.Get(grouper)
                .Setup(g => g.GroupRelatedZips(It.IsAny<IDictionary<string, IKaVESet<string>>>()))
                .Returns<IDictionary<string, IKaVESet<string>>>(
                    d =>
                    {
                        Assert.Null(_actualGroupedIds);
                        lock (_lock)
                        {
                            _actualGroupedIds = d;
                        }
                        return GetExpectedGroups();
                    });
            return grouper;
        }

        private IGroupMerger GroupMergerFactory(int taskId)
        {
            var groupMerger = Mock.Of<IGroupMerger>();
            Mock.Get(groupMerger)
                .Setup(gm => gm.Merge(It.IsAny<IKaVESet<string>>()))
                .Returns<IKaVESet<string>>(
                    zipGroup =>
                    {
                        lock (_lock)
                        {
                            _actualGroups.Add(zipGroup);
                        }
                        return zipGroup.First();
                    });
            return groupMerger;
        }

        private ICleaner CleanerFactory(int taskId)
        {
            var cleaner = Mock.Of<ICleaner>();
            Mock.Get(cleaner)
                .Setup(c => c.Clean(It.IsAny<string>()))
                .Callback<string>(
                    zip =>
                    {
                        lock (_lock)
                        {
                            _actualCleanedZips.Add(zip);
                        }
                    });
            return cleaner;
        }

        [Test]
        public void AssertAllValuesAreCompletelyProcessed()
        {
            _sut.Run();

            var e1 = GetExpectedZips().Select(zip => "<dirIn>\\" + zip);
            CollectionAssert.AreEquivalent(e1, _actualProcessedZips);
            var e2 = GetExpectedIds();
            CollectionAssert.AreEquivalent(e2, _actualGroupedIds);
            var e3 = GetExpectedGroups();
            CollectionAssert.AreEquivalent(e3, _actualGroups);
            var e4 = GetExpectedCleanedZips();
            CollectionAssert.AreEquivalent(e4, _actualCleanedZips);
        }

        [Test]
        public void LoggerTest()
        {
            Mock.Get(_io).Verify(io => io.GetFullPath_In(""), Times.Exactly(1));
            Mock.Get(_io).Verify(io => io.GetFullPath_Merged(""), Times.Exactly(1));
            Mock.Get(_io).Verify(io => io.GetFullPath_Out(""), Times.Exactly(1));
            Mock.Get(_log)
                .Verify(l => l.Init(NumWorker, @"<dirIn>\", @"<dirMerged>\", @"<dirOut>\"), Times.Exactly(1));

            AssertAllValuesAreCompletelyProcessed();

            Mock.Get(_log).Verify(l => l.ReadingIds(NumGroups*NumZipsPerGroup));
            for (var i = 0; i < NumWorker; i++)
            {
                var workerId = i;
                Mock.Get(_log).Verify(l => l.StartWorkerReadIds(workerId), Times.Exactly(1));
                Mock.Get(_log).Verify(l => l.StopWorkerReadIds(workerId), Times.Exactly(1));
                Mock.Get(_log).Verify(l => l.StartWorkerMergeGroup(workerId), Times.Exactly(1));
                Mock.Get(_log).Verify(l => l.StopWorkerMergeGroup(workerId), Times.Exactly(1));
                Mock.Get(_log).Verify(l => l.StartWorkerCleanZip(workerId), Times.Exactly(1));
                Mock.Get(_log).Verify(l => l.StopWorkerCleanZip(workerId), Times.Exactly(1));
            }
            foreach (var zip in GetExpectedZips())
            {
                var zip1 = zip;
                Mock.Get(_log).Verify(l => l.ReadIds(It.IsAny<int>(), zip1), Times.Exactly(1));
            }

            Mock.Get(_log).Verify(l => l.GroupZipsByIds(), Times.Exactly(1));

            Mock.Get(_log).Verify(l => l.MergeGroups(NumGroups), Times.Exactly(1));
            Mock.Get(_log).Verify(l => l.MergeGroup(It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(NumGroups));

            Mock.Get(_log).Verify(l => l.Cleaning(NumGroups), Times.Exactly(1));
            foreach (var zip in GetExpectedCleanedZips())
            {
                var zip1 = zip;
                Mock.Get(_log).Verify(l => l.CleanZip(It.IsAny<int>(), zip1), Times.Exactly(1));
            }
        }

        [Test]
        public void ErrorsInZipReadingAreReportedAndExcecutionContinues()
        {
            var ex = CaptureException();

            _sut = new MultiThreadedPreprocessing(
                _io,
                _log,
                NumWorker,
                taskId => new CrashingIdReader(ex),
                CreateGrouper(),
                GroupMergerFactory,
                CleanerFactory);

            _sut.Run();

            Assert.AreSame(ex, _lastLoggedException);
        }

        [Test]
        public void ErrorsInZipMergingAreReportedAndExcecutionContinues()
        {
            var ex = CaptureException();

            _sut = new MultiThreadedPreprocessing(
                _io,
                _log,
                NumWorker,
                IdReaderFactory,
                CreateGrouper(),
                taskId => new CrashingGroupMerger(ex),
                CleanerFactory);

            _sut.Run();

            Assert.AreSame(ex, _lastLoggedException);
        }

        [Test]
        public void ErrorsInZipCleaningAreReportedAndExcecutionContinues()
        {
            var ex = CaptureException();

            _sut = new MultiThreadedPreprocessing(
                _io,
                _log,
                NumWorker,
                IdReaderFactory,
                CreateGrouper(),
                GroupMergerFactory,
                taskId => new CrashingCleaner(ex));

            _sut.Run();

            Assert.AreSame(ex, _lastLoggedException);
        }

        private static Exception CaptureException()
        {
            try
            {
                throw new Exception("test exception");
            }
            catch (Exception e)
            {
                return e;
            }
        }

        private class CrashingIdReader : IIdReader
        {
            private readonly Exception _e;

            public CrashingIdReader(Exception e)
            {
                _e = e;
            }

            public IKaVESet<string> Read(string zip)
            {
                throw _e;
            }
        }

        private class CrashingGroupMerger : IGroupMerger
        {
            private readonly Exception _e;

            public CrashingGroupMerger(Exception e)
            {
                _e = e;
            }

            public string Merge(IKaVESet<string> relZips)
            {
                throw _e;
            }
        }

        private class CrashingCleaner : ICleaner
        {
            private readonly Exception _e;

            public CrashingCleaner(Exception e)
            {
                _e = e;
            }

            public void Dispose() {}

            public void Clean(string relZip)
            {
                throw _e;
            }
        }
    }
}
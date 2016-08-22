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
        private readonly object _lock = new object();
        private IPreprocessingIo _io;
        private IMultiThreadedPreprocessingLogger _log;

        MultiThreadedPreprocessing _sut;

        private IKaVESet<string> _actualProcessedZips;
        private IDictionary<string, IKaVESet<string>> _actualGroupedIds;
        private IList<IKaVESet<string>> _actualGroups;
        private IKaVEList<string> _actualCleanedZips;

        [SetUp]
        public void Setup()
        {
            _actualProcessedZips = Sets.NewHashSet<string>();
            _actualGroupedIds = null;
            _actualGroups = Lists.NewList<IKaVESet<string>>();
            _actualCleanedZips = Lists.NewList<string>();

            _io = Mock.Of<IPreprocessingIo>();
            Mock.Get(_io).Setup(io => io.FindRelativeZipPaths()).Returns(GetExpectedZips());
            _log = Mock.Of<IMultiThreadedPreprocessingLogger>();


            _sut = new MultiThreadedPreprocessing(
                _io,
                _log,
                4,
                IdReaderFactory,
                CreateGrouper(),
                GroupMergerFactory,
                CleanerFactory);
        }

        private IKaVESet<string> GetExpectedZips()
        {
            var zips = Sets.NewHashSet<string>();
            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < 100; j++)
                {
                    zips.Add("zip_" + i + "_" + j);
                }
            }
            return zips;
        }

        private IDictionary<string, IKaVESet<string>> GetExpectedIds()
        {
            var d = new Dictionary<string, IKaVESet<string>>();

            foreach (var zip in GetExpectedZips())
            {
                var irrelevantId = zip + "_..."; // no overlap ever
                var overlappingId = zip.Substring(0, 5);
                d[zip] = Sets.NewHashSet(irrelevantId, overlappingId);
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
                    return ids[zip];
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

            var e1 = GetExpectedZips();
            Assert.IsTrue(e1.Equals(_actualProcessedZips));
            CollectionAssert.AreEquivalent(e1, _actualProcessedZips);
            var e2 = GetExpectedIds();
            CollectionAssert.AreEquivalent(e2, _actualGroupedIds);
            var e3 = GetExpectedGroups();
            CollectionAssert.AreEquivalent(e3, _actualGroups);
            var e4 = GetExpectedCleanedZips();
            CollectionAssert.AreEquivalent(e4, _actualCleanedZips);
        }
    }
}
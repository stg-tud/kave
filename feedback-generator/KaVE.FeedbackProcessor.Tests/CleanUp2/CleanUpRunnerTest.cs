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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Collections;
using KaVE.RS.SolutionAnalysis.CleanUp;
using KaVE.RS.SolutionAnalysis.CleanUp.Filters;
using Moq;
using NUnit.Framework;

namespace KaVE.RS.SolutionAnalysis.Tests.CleanUp
{
    internal class CleanUpRunnerTest
    {
        #region setup and helper

        private CleanUpRunner _sut;
        private ICleanUpIo _io;
        private ICleanUpLogger _log;

        private IDictionary<string, IList<IDEEvent>> _inEvents;
        private IDictionary<string, IEnumerable<IDEEvent>> _outEvents;
        private IDictionary<string, IDictionary<string, int>> _intermediateResults;

        [SetUp]
        public void Setup()
        {
            _inEvents = new Dictionary<string, IList<IDEEvent>>();
            _outEvents = new Dictionary<string, IEnumerable<IDEEvent>>();
            _intermediateResults = new Dictionary<string, IDictionary<string, int>>();

            _io = Mock.Of<ICleanUpIo>();
            Mock.Get(_io).Setup(io => io.GetZips()).Returns(() => Lists.NewListFrom(_inEvents.Keys));
            Mock.Get(_io)
                .Setup(io => io.ReadZip(It.IsAny<string>()))
                .Returns<string>(zip => _inEvents[zip]);
            Mock.Get(_io)
                .Setup(io => io.WriteZip(It.IsAny<IEnumerable<IDEEvent>>(), It.IsAny<string>()))
                .Callback<IEnumerable<IDEEvent>, string>(
                    (events, zip) => { _outEvents[zip] = Lists.NewListFrom(events); });

            _log = Mock.Of<ICleanUpLogger>();
            Mock.Get(_log)
                .Setup(l => l.IntermediateResult(It.IsAny<string>(), It.IsAny<IDictionary<string, int>>()))
                .Callback<string, IDictionary<string, int>>((zip, counts) => { _intermediateResults[zip] = counts; });

            _sut = new CleanUpRunner(_io, _log);
        }

        private void Add(string fileName, params IDEEvent[] es)
        {
            _inEvents[fileName] = Lists.NewListFrom(es);
        }

        private static IDEEvent E(string id, int timeOffset)
        {
            return new CommandEvent
            {
                CommandId = id,
                TriggeredAt = DateTime.MinValue.AddSeconds(timeOffset)
            };
        }

        private void AssertEvents(string fileName, params IDEEvent[] expectedEvents)
        {
            Assert.That(_outEvents.ContainsKey(fileName));
            CollectionAssert.AreEqual(expectedEvents, _outEvents[fileName]);
        }

        #endregion

        [Test]
        public void NoFiltersByDefault()
        {
            Assert.NotNull(_sut.Filters);
            CollectionAssert.AreEqual(new IFilter[] {}, _sut.Filters);
        }

        [Test]
        public void DuplicatesAreRemoved()
        {
            Add("a", E("a", 1), E("a", 1));

            _sut.Run();

            AssertEvents("a", E("a", 1));
        }

        [Test]
        public void EventsAreOrdered()
        {
            Add("a", E("a", 2), E("b", 1));

            _sut.Run();

            AssertEvents("a", E("b", 1), E("a", 2));
        }

        [Test]
        public void FiltersCanBeAddedAndTheyAreExecuted()
        {
            _sut.Filters.Add(new TestFilter("b"));

            Add("a", E("a", 1), E("b", 2), E("c", 3));

            _sut.Run();

            AssertEvents("a", E("a", 1), E("c", 3));
        }

        [Test]
        public void IntegrationExample()
        {
            _sut.Filters.Add(new TestFilter("b"));

            Add("a", E("a", 3), E("b", 2), E("c", 1), E("a", 3));
            Add("b", E("d", 1));

            _sut.Run();

            AssertEvents("a", E("c", 1), E("a", 3));
            AssertEvents("b", E("d", 1));

            Mock.Get(_log).Verify(l => l.FoundZips(new[] {"a", "b"}));

            Mock.Get(_log).Verify(l => l.ReadingZip("a"), Times.Exactly(1));
            Mock.Get(_log).Verify(l => l.ReadingZip("b"), Times.Exactly(1));
            Mock.Get(_log).Verify(l => l.ApplyingFilters(), Times.Exactly(2));
            Mock.Get(_log).Verify(l => l.ApplyingFilter("command filter: b"), Times.Exactly(2));
            Mock.Get(_log).Verify(l => l.RemovingDuplicates(), Times.Exactly(2));
            Mock.Get(_log).Verify(l => l.OrderingEvents(), Times.Exactly(2));
            Mock.Get(_log).Verify(l => l.WritingEvents(), Times.Exactly(2));
            Mock.Get(_log)
                .Verify(l => l.IntermediateResult("a", It.IsAny<IDictionary<string, int>>()), Times.Exactly(1));
            Mock.Get(_log)
                .Verify(l => l.IntermediateResult("b", It.IsAny<IDictionary<string, int>>()), Times.Exactly(1));

            CollectionAssert.AreEquivalent(Res(4, 3, 2, 2), _intermediateResults["a"]);
            CollectionAssert.AreEquivalent(Res(1, 1, 1, 1), _intermediateResults["b"]);
        }

        private static IDictionary<string, int> Res(int numBefore,
            int numAfterFilter,
            int numAfterDuplicate,
            int numAfterOrdering)
        {
            return new Dictionary<string, int>
            {
                {"before applying any filter", numBefore},
                {"after applying 'command filter: b'", numAfterFilter},
                {"after removing duplicates", numAfterDuplicate},
                {"after ordering", numAfterOrdering}
            };
        }

        private class TestFilter : BaseFilter
        {
            private readonly string _filterId;

            public TestFilter(string filterId)
            {
                _filterId = filterId;
            }

            public override string Name
            {
                get { return "command filter: " + _filterId; }
            }

            public override Func<IDEEvent, bool> Func
            {
                get
                {
                    return e =>
                    {
                        var ce = e as CommandEvent;
                        if (ce != null)
                        {
                            return !ce.CommandId.Equals(_filterId);
                        }
                        return true;
                    };
                }
            }
        }
    }
}
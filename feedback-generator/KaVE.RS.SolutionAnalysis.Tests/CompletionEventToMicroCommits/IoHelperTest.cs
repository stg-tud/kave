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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.IO.Archives;
using KaVE.RS.SolutionAnalysis.CompletionEventToMicroCommits;
using NUnit.Framework;

namespace KaVE.RS.SolutionAnalysis.Tests.CompletionEventToMicroCommits
{
    internal class IoHelperTest
    {
        private string _dirEvents;
        private string _dirHistories;

        private IoHelper _sut;

        [SetUp]
        public void Setup()
        {
            _dirEvents = CreateTempDir();
            _dirHistories = CreateTempDir();
            _sut = new IoHelper(_dirEvents, _dirHistories);
        }

        [TearDown]
        public void Teardown()
        {
            if (Directory.Exists(_dirEvents))
            {
                Directory.Delete(_dirEvents, true);
            }
            if (Directory.Exists(_dirHistories))
            {
                Directory.Delete(_dirHistories, true);
            }
        }

        private static string CreateTempDir()
        {
            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(path);
            return path;
        }

        [Test]
        public void FindExports()
        {
            var f0 = Path.Combine(_dirEvents, "0.zip");
            var sub = Path.Combine(_dirEvents, "sub");
            Directory.CreateDirectory(sub);
            var f1 = Path.Combine(sub, "1.zip");

            File.Create(f0).Close();
            File.Create(f1).Close();

            var actuals = _sut.FindExports();
            var expecteds = Lists.NewList(f0, f1);
            CollectionAssert.AreEquivalent(expecteds, actuals);
        }

        [Test]
        public void ReadEvents_HappyPath()
        {
            var e1 = new CommandEvent
            {
                TriggeredAt = DateTime.Now,
                ActiveDocument = Names.Document("... somefile.cs")
            };
            var e2 = new CompletionEvent
            {
                TriggeredAt = DateTime.Now,
                ActiveDocument = Names.Document("... otherfile.cs")
            };
            Write(e1, e2);
            AssertEvents(e1, e2);
        }

        [Test]
        public void ReadCompletionEvents_HappyPath()
        {
            var e1 = new CommandEvent
            {
                TriggeredAt = DateTime.Now,
                ActiveDocument = Names.Document("... somefile.cs")
            };
            var e2 = new CompletionEvent
            {
                TriggeredAt = DateTime.Now,
                ActiveDocument = Names.Document("... otherfile.cs")
            };
            Write(e1, e2);
            AssertCompletionEvents(e2);
        }

        [Test]
        public void ReadCompletionEvents_Null()
        {
            var e1 = new CompletionEvent
            {
                TriggeredAt = DateTime.Now,
                ActiveDocument = Names.Document("... somefile.cs")
            };
            Write(e1, null);
            AssertCompletionEvents(e1);
        }

        [Test]
        public void ReadCompletionEvents_NoTriggerInfo()
        {
            var e1 = new CompletionEvent
            {
                ActiveDocument = Names.Document("... somefile.cs")
            };
            Write(e1);
            AssertCompletionEvents();
        }

        [Test]
        public void ReadCompletionEvents_NoCSharpFile()
        {
            var e1 = new CompletionEvent
            {
                TriggeredAt = DateTime.Now,
                ActiveDocument = Names.Document("... somefile.xml")
            };
            Write(e1);
            AssertCompletionEvents();
        }

        [Test]
        public void AddTuples()
        {
            var q = new Query
            {
                type = new CoReTypeName("Ln/T")
            };
            var expected = Tuple.Create(q, q);

            _sut.OpenCache();
            _sut.AddTuple(expected);
            _sut.CloseCache();

            using (var ra = new ReadingArchive(Path.Combine(_dirHistories, "Ln", "T", "0.zip")))
            {
                var ts = ra.GetAll<Tuple<Query, Query>>();

                Assert.AreEqual(1, ts.Count);
                Assert.AreEqual(expected, ts[0]);
            }
        }

        private void Write(params IDEEvent[] es)
        {
            var fullName = Path.Combine(_dirEvents, "x.zip");
            using (var wa = new WritingArchive(fullName))
            {
                wa.AddAll(es);
            }
        }

        private void AssertEvents(params IDEEvent[] es)
        {
            var fullName = Path.Combine(_dirEvents, "x.zip");
            var actuals = _sut.ReadEvents(fullName);
            var expecteds = Lists.NewListFrom(es);
            Assert.AreEqual(expecteds, actuals);
        }

        private void AssertCompletionEvents(params IDEEvent[] es)
        {
            var fullName = Path.Combine(_dirEvents, "x.zip");
            var actuals = _sut.ReadCompletionEvents(fullName);
            var expecteds = Lists.NewListFrom(es);
            Assert.AreEqual(expecteds, actuals);
        }
    }
}
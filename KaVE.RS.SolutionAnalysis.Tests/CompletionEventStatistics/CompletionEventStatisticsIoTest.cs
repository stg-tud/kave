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
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.IO.Archives;
using KaVE.RS.SolutionAnalysis.CompletionEventStatistics;
using NUnit.Framework;

namespace KaVE.RS.SolutionAnalysis.Tests.CompletionEventStatistics
{
    internal class CompletionEventStatisticsIoTest
    {
        #region setup and helpers

        private CompletionEventStatisticsIo _sut;
        private string _dirIn;

        [SetUp]
        public void Setup()
        {
            _dirIn = CreateTempDir();
            _sut = new CompletionEventStatisticsIo(_dirIn);
        }

        [TearDown]
        public void Teardown()
        {
            if (Directory.Exists(_dirIn))
            {
                Directory.Delete(_dirIn, true);
            }
        }

        private static string CreateTempDir()
        {
            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(path);
            return path;
        }

        private static IDEEvent CompletionEvent(TerminationState ts, IName name)
        {
            return new CompletionEvent
            {
                // this is invalid but Ok for this test!
                ActiveDocument = Names.Document(Guid.NewGuid() + " "),
                TerminatedState = ts,
                Selections =
                {
                    new ProposalSelection
                    {
                        Proposal = new Proposal
                        {
                            Name = name
                        }
                    }
                }
            };
        }

        private static IDEEvent OtherEvent()
        {
            return new ActivityEvent
            {
                // this is invalid but Ok for this test!
                ActiveDocument = Names.Document(Guid.NewGuid() + " x")
            };
        }

        private void AddFile(string fileName, params IDEEvent[] events)
        {
            var fullName = Path.Combine(_dirIn, fileName);
            var dir = Path.GetDirectoryName(fullName);
            if (dir != null)
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }

            using (var wa = new WritingArchive(fullName))
            {
                foreach (var e in events)
                {
                    wa.Add(e);
                }
            }
        }

        #endregion

        [Test]
        public void FindsAllZips()
        {
            AddFile("a.zip", OtherEvent());
            AddFile(Path.Combine("b", "c.zip"), OtherEvent());

            var expected = Lists.NewList("a.zip", Path.Combine("b", "c.zip"));
            var actual = _sut.FindZips();
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void OnlyReadsAppliedEventsWithMethods()
        {
            const TerminationState applied = TerminationState.Applied;
            const TerminationState cancelled = TerminationState.Cancelled;

            var x = Names.General("x");
            var m = Names.Method("[T,P] [T,P].M()");

            var appliedAndMethod = CompletionEvent(applied, m);

            AddFile(
                "a.zip",
                OtherEvent(),
                CompletionEvent(applied, x),
                CompletionEvent(cancelled, m),
                appliedAndMethod);

            var expected = Lists.NewList(appliedAndMethod);
            var actual = _sut.FindAppliedCompletionEvents("a.zip");
            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}
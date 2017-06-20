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
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Utils.Collections;
using KaVE.RS.SolutionAnalysis.CompletionEventToMicroCommits;
using Moq;
using NUnit.Framework;

namespace KaVE.RS.SolutionAnalysis.Tests.CompletionEventToMicroCommits
{
    internal class CompletionEventToMicroCommitRunnerTest
    {
        private IIoHelper _ioHelper;
        private IMicroCommitGenerator _microCommitGenerator;

        private CompletionEventToMicroCommitRunner _sut;

        [SetUp]
        public void Setup()
        {
            _microCommitGenerator = Mock.Of<IMicroCommitGenerator>();
            _ioHelper = Mock.Of<IIoHelper>();

            _sut = new CompletionEventToMicroCommitRunner(_microCommitGenerator, _ioHelper);
        }

        [Test]
        public void VerifyTheHappyPath()
        {
            Mock.Get(_ioHelper).Setup(io => io.FindExports()).Returns(new List<string> {"a", "b"});

            var ce1 = new CompletionEvent {Id = "1"};
            var ce2 = new CompletionEvent {Id = "2"};
            var ce3 = new CompletionEvent {Id = "3"};
            var ce4 = new CompletionEvent {Id = "4"};

            var ctx1 = new Context {SST = new SST {PartialClassIdentifier = "1"}};
            var ctx2 = new Context {SST = new SST {PartialClassIdentifier = "2"}};
            var ctx3 = new Context {SST = new SST {PartialClassIdentifier = "3"}};
            var ctx4 = new Context {SST = new SST {PartialClassIdentifier = "4"}};

            Mock.Get(_ioHelper)
                .Setup(io => io.ReadCompletionEvents("a"))
                .Returns(Lists.NewList(ce1, ce3, ce2));

            Mock.Get(_ioHelper)
                .Setup(io => io.ReadCompletionEvents("b"))
                .Returns(Lists.NewList(ce4));

            Mock.Get(_microCommitGenerator).Setup(t => t.GetTemporalIndex(ce1)).Returns("k1");
            Mock.Get(_microCommitGenerator).Setup(t => t.GetTemporalIndex(ce2)).Returns("k1");
            Mock.Get(_microCommitGenerator).Setup(t => t.GetTemporalIndex(ce3)).Returns("k2");
            Mock.Get(_microCommitGenerator).Setup(t => t.GetTemporalIndex(ce4)).Returns("k2");

            Mock.Get(_microCommitGenerator)
                .Setup(t => t.FindFirstAndLast(Lists.NewList(ce1, ce2)))
                .Returns(Tuple.Create(ctx1, ctx2));
            Mock.Get(_microCommitGenerator)
                .Setup(t => t.FindFirstAndLast(Lists.NewList(ce3, ce4)))
                .Returns(Tuple.Create(ctx3, ctx4));

            var t1 = CreateTuple("a");
            var t2 = CreateTuple("b");

            Mock.Get(_microCommitGenerator)
                .Setup(t => t.GenerateTuples(ctx1, ctx2))
                .Returns(new List<Tuple<Query, Query>> {t1});
            Mock.Get(_microCommitGenerator)
                .Setup(t => t.GenerateTuples(ctx3, ctx4))
                .Returns(new List<Tuple<Query, Query>> {t2});


            _sut.Run();

            Mock.Get(_ioHelper).Verify(io => io.ReadCompletionEvents("a"), Times.Once);
            Mock.Get(_ioHelper).Verify(io => io.ReadCompletionEvents("b"), Times.Once);
            foreach (var ce in new[] {ce1, ce2, ce3, ce4})
            {
                Mock.Get(_microCommitGenerator).Verify(t => t.GetTemporalIndex(ce), Times.Once);
            }

            Mock.Get(_microCommitGenerator)
                .Verify(t => t.FindFirstAndLast(Lists.NewList(ce1, ce2)), Times.Once());
            Mock.Get(_microCommitGenerator)
                .Verify(t => t.FindFirstAndLast(Lists.NewList(ce3, ce4)), Times.Once());

            Mock.Get(_microCommitGenerator).Verify(t => t.GenerateTuples(ctx1, ctx2), Times.Once());
            Mock.Get(_microCommitGenerator).Verify(t => t.GenerateTuples(ctx3, ctx4), Times.Once());

            Mock.Get(_ioHelper).Verify(io => io.AddTuple(t1));
            Mock.Get(_ioHelper).Verify(io => io.AddTuple(t2));

            Mock.Get(_ioHelper).Verify(io => io.OpenCache(), Times.Once());
            Mock.Get(_ioHelper).Verify(io => io.CloseCache(), Times.Once());
        }

        private Tuple<Query, Query> CreateTuple(string s)
        {
            return Tuple.Create<Query, Query>(null, null);
        }

        [Test]
        public void OnlyOneCompletionEventInFile()
        {
            Mock.Get(_ioHelper).Setup(io => io.FindExports()).Returns(new List<string> {"a"});

            var ce1 = new CompletionEvent {Id = "1"};

            Mock.Get(_ioHelper)
                .Setup(io => io.ReadCompletionEvents("a"))
                .Returns(Lists.NewList(ce1));

            Mock.Get(_microCommitGenerator).Setup(t => t.GetTemporalIndex(ce1)).Returns("k1");

            _sut.Run();

            Mock.Get(_microCommitGenerator)
                .Verify(t => t.FindFirstAndLast(It.IsAny<List<CompletionEvent>>()), Times.Never);
        }
    }
}
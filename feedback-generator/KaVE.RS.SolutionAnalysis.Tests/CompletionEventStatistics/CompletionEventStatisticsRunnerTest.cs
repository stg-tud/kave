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
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.Impl.v0;
using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Utils.Collections;
using KaVE.RS.SolutionAnalysis.CompletionEventStatistics;
using Moq;
using NUnit.Framework;

namespace KaVE.RS.SolutionAnalysis.Tests.CompletionEventStatistics
{
    internal class CompletionEventStatisticsRunnerTest
    {
        private ICompletionEventStatisticsIo _io;
        private ICompletionEventStatisticsLogger _log;
        private CompletionEventStatisticsRunner _sut;

        [SetUp]
        public void Setup()
        {
            _io = Mock.Of<ICompletionEventStatisticsIo>();
            _log = Mock.Of<ICompletionEventStatisticsLogger>();
            _sut = new CompletionEventStatisticsRunner(_io, _log);
        }

        [Test]
        public void Run()
        {
            var m1 = MethodName.Get("[T,P] [T2,P].M()");
            var m2 = MethodName.Get("[T,P] [T2,P].M2()");
            var m3 = MethodName.Get("[T,P] [T3,P].M3()");

            Mock.Get(_io).Setup(io => io.FindZips()).Returns(Lists.NewList("a", "b"));

            Mock.Get(_io)
                .Setup(io => io.FindAppliedCompletionEvents("a"))
                .Returns(Lists.NewList(Ce(Name.Get("x")), Ce(m1)));
            Mock.Get(_io)
                .Setup(io => io.FindAppliedCompletionEvents("b"))
                .Returns(Lists.NewList(Ce(m2), Ce(Name.Get("y")), Ce(m3)));

            _sut.Run();

            Mock.Get(_log).Verify(l => l.FoundZips(It.IsAny<IList<string>>()));
            Mock.Get(_log).Verify(l => l.StartingZip("a"));
            Mock.Get(_log).Verify(l => l.StartingZip("b"));
            Mock.Get(_log).Verify(l => l.DoneWithZip(), Times.Exactly(2));
            Mock.Get(_log).Verify(l => l.FoundOtherProposal(), Times.Exactly(2));
            Mock.Get(_log).Verify(l => l.Store(m1));
            Mock.Get(_log).Verify(l => l.Store(m2));
            Mock.Get(_log).Verify(l => l.Store(m3));
            Mock.Get(_log).Verify(l => l.Done());
        }

        private ICompletionEvent Ce(IName n)
        {
            return new CompletionEvent
            {
                Selections =
                {
                    new ProposalSelection
                    {
                        Proposal = new Proposal
                        {
                            Name = n
                        }
                    }
                }
            };
        }
    }
}
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

using System.Linq;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Utils.Collections;
using KaVE.RS.SolutionAnalysis.CompletionEventStatistics;
using NUnit.Framework;

namespace KaVE.RS.SolutionAnalysis.Tests.CompletionEventStatistics
{
    internal class CompletionEventStatisticsLoggerTest
    {
        private CompletionEventStatisticsLogger _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new CompletionEventStatisticsLogger();
        }

        [Test]
        public void HappyPath()
        {
            var zips = new[] {"a", "b"}.ToList();
            _sut.FoundZips(zips);

            foreach (var zip in zips)
            {
                _sut.StartingZip(zip);
                _sut.FoundAppliedCompletionEvents(Lists.NewList<ICompletionEvent>());
                _sut.Store(MethodName.Get("[T,P] [T1,P].M1()"));
                _sut.Store(MethodName.Get("[T,P] [T1,P].M2()"));
                _sut.Store(MethodName.Get("[T,P] [T2,P].M3()"));
                _sut.FoundOtherProposal();

                _sut.DoneWithZip();
            }

            _sut.Done();
        }
    }
}
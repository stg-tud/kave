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

using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Naming.Impl.v0.IDEComponents;
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.RS.SolutionAnalysis.CleanUp.Filters;
using NUnit.Framework;

namespace KaVE.RS.SolutionAnalysis.Tests.SortByUser.Filters
{
    internal class InvalidCompletionEventFilterTest
    {
        private InvalidCompletionEventFilter _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new InvalidCompletionEventFilter();
        }

        private static CompletionEvent CreateValidCompletionEvent()
        {
            return new CompletionEvent
            {
                ActiveDocument = DocumentName.Get(@"CSharp \path\to\abc.cs"),
                Context2 = new Context
                {
                    SST = new SST
                    {
                        EnclosingType = TypeName.Get("T,P")
                    }
                }
            };
        }

        [Test]
        public void HasName()
        {
            Assert.AreEqual(typeof (InvalidCompletionEventFilter).Name, _sut.Name);
        }

        [Test]
        public void KeepsOtherEvents()
        {
            var e = new CommandEvent();
            Assert.True(_sut.Func(e));
        }

        [Test]
        public void KeepsValidCompletionEvents()
        {
            var e = CreateValidCompletionEvent();
            Assert.True(_sut.Func(e));
        }

        [Test]
        public void RemovesEventsWithWrongFileExtension()
        {
            var e = CreateValidCompletionEvent();
            e.ActiveDocument = DocumentName.Get(@"CSharp \path\to\abc.txt");
            Assert.False(_sut.Func(e));
        }

        [Test]
        public void RemovesEventsWithDefaultContext()
        {
            var e = CreateValidCompletionEvent();
            e.Context2 = new Context();
            Assert.False(_sut.Func(e));
        }
    }
}
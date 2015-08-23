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
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Names.VisualStudio;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.IO;
using KaVE.Commons.Utils.IO.Archives;
using Moq;
using NUnit.Framework;

namespace KaVE.RS.SolutionAnalysis.Tests
{
    internal class CompletionEventFilterTest
    {
        private IIoUtils _io;
        private CompletionEventFilterLogger _logger;
        private CompletionEventFilter _sut;
        private IReadingArchive _ra;
        private IWritingArchive _wa;
        private IDEEvent _e1;
        private IDEEvent _e2;
        private IDEEvent _e3;
        private IDEEvent _e4;
        private IDEEvent _e5;
        private IDEEvent _e6;
        private IDEEvent _e7;
        private List<IDEEvent> _storedEvents;

        [SetUp]
        public void SetUp()
        {
            _io = Mock.Of<IIoUtils>();

            Mock.Get(_io)
                .Setup(io => io.GetFilesRecursive(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new[] {@"C:\from\a\a.zip"});

            _ra = Mock.Of<IReadingArchive>();
            _wa = Mock.Of<IWritingArchive>();

            Mock.Get(_io).Setup(io => io.ReadArchive(@"C:\from\a\a.zip")).Returns(_ra);
            Mock.Get(_io).Setup(io => io.CreateArchive(@"C:\to\a\a.zip")).Returns(_wa);

            _logger = Mock.Of<CompletionEventFilterLogger>();

            _e1 = CreateNonCompletionEvent();
            _e2 = CreateCompletionEvent();
            _e3 = CreateCompletionEvent_NoCSharpFile();
            _e4 = CreateCompletionEvent_Incomplete_NoSessionId();
            _e5 = CreateCompletionEvent_Incomplete_NoTriggerTime();
            _e6 = CreateCompletionEvent_NoMethodDeclarations();
            _e7 = CreateCompletionEvent_NoTriggerPoint();

            Mock.Get(_ra).Setup(ra => ra.Count).Returns(7);
            Mock.Get(_ra)
                .SetupSequence(ra => ra.HasNext())
                .Returns(true)
                .Returns(true)
                .Returns(true)
                .Returns(true)
                .Returns(true)
                .Returns(true)
                .Returns(true)
                .Returns(false);
            Mock.Get(_ra)
                .SetupSequence(ra => ra.GetNext<IDEEvent>())
                .Returns(_e1)
                .Returns(_e2)
                .Returns(_e3)
                .Returns(_e4)
                .Returns(_e5)
                .Returns(_e6)
                .Returns(_e7);

            _storedEvents = new List<IDEEvent>();
            Mock.Get(_wa).Setup(wa => wa.Add(It.IsAny<IDEEvent>())).Callback<IDEEvent>(e => _storedEvents.Add(e));
        }

        [Test]
        public void HappyPath_KeepsNoTrigger()
        {
            var option = CompletionEventFilter.NoTriggerPointOption.Keep;
            CreateSut(option);

            VerifyIo();
            VerifyGeneralLogger();

            Assert.AreEqual(2, _storedEvents.Count);
            Assert.AreEqual(RemoveProposals(_e2), _storedEvents[0]);
            Assert.AreEqual(RemoveProposals(_e7), _storedEvents[1]);
            Mock.Get(_wa).Verify(wa => wa.Dispose());

            Mock.Get(_logger).Verify(l => l.FoundZips(1, option));
            Mock.Get(_logger).Verify(l => l.Finish(7, 3, 1, 1, 1, 2, option));
        }

        [Test]
        public void HappyPath_RemovesNoTrigger()
        {
            var option = CompletionEventFilter.NoTriggerPointOption.Remove;
            CreateSut(option);

            VerifyIo();
            VerifyGeneralLogger();

            Assert.AreEqual(1, _storedEvents.Count);
            Assert.AreEqual(RemoveProposals(_e2), _storedEvents[0]);
            Mock.Get(_wa).Verify(wa => wa.Dispose());

            Mock.Get(_logger).Verify(l => l.FoundZips(1, option));
            Mock.Get(_logger).Verify(l => l.Finish(7, 3, 1, 1, 1, 1, option));
        }

        private void CreateSut(CompletionEventFilter.NoTriggerPointOption noTriggerPointOption)
        {
            _sut = new CompletionEventFilter(
                @"C:\from\",
                @"C:\to\",
                noTriggerPointOption,
                _io,
                _logger);

            _sut.Run();
        }

        private void VerifyGeneralLogger()
        {
            Mock.Get(_logger).Verify(l => l.ProgressZip(1, 1, @"C:\from\a\a.zip", @"C:\to\a\a.zip"));
            Mock.Get(_logger).Verify(l => l.FoundEvents(7));
            Mock.Get(_logger).Verify(l => l.ProgressEvent('.'), Times.Exactly(1));
            Mock.Get(_logger).Verify(l => l.ProgressEvent(':'), Times.Exactly(3));
            Mock.Get(_logger).Verify(l => l.ProgressEvent('|'), Times.Exactly(1));
            Mock.Get(_logger).Verify(l => l.ProgressEvent('o'), Times.Exactly(1));
            Mock.Get(_logger).Verify(l => l.ProgressEvent('x'), Times.Exactly(1));
        }

        private void VerifyIo()
        {
            Mock.Get(_io).Verify(io => io.GetFilesRecursive(@"C:\from\", "*.zip"));
            Mock.Get(_io).Verify(io => io.ReadArchive(@"C:\from\a\a.zip"));
            Mock.Get(_io).Verify(io => io.CreateDirectory(@"C:\to\a"));
            Mock.Get(_io).Verify(io => io.CreateArchive(@"C:\to\a\a.zip"));
        }

        private static IDEEvent CreateNonCompletionEvent()
        {
            var e = new CommandEvent();
            AddBasicInformation(e);
            return e;
        }

        private static CompletionEvent CreateCompletionEvent()
        {
            var e = new CompletionEvent();
            AddBasicInformation(e);
            e.Context2 = CreateContext(
                new ContinueStatement(),
                new ExpressionStatement {Expression = new CompletionExpression()});
            e.ProposalCollection = new ProposalCollection
            {
                new Proposal()
            };
            return e;
        }

        private static IDEEvent CreateCompletionEvent_NoCSharpFile()
        {
            var e = CreateCompletionEvent();
            e.ActiveDocument = DocumentName.Get("... blabla.xml");
            return e;
        }

        private static void AddBasicInformation(IDEEvent e)
        {
            e.IDESessionUUID = "SomeUID";
            e.TriggeredAt = DateTime.Now;
            e.ActiveDocument = DocumentName.Get("... blabla.cs");
        }

        private static Context CreateContext(params IStatement[] stmts)
        {
            return new Context
            {
                SST = new SST
                {
                    Methods =
                    {
                        new MethodDeclaration
                        {
                            Body = Lists.NewList(stmts)
                        }
                    }
                }
            };
        }

        private static IDEEvent CreateCompletionEvent_Incomplete_NoSessionId()
        {
            var ce = CreateCompletionEvent();
            ce.IDESessionUUID = null;
            return ce;
        }

        private static IDEEvent CreateCompletionEvent_Incomplete_NoTriggerTime()
        {
            var ce = CreateCompletionEvent();
            ce.TriggeredAt = null;
            return ce;
        }

        private static IDEEvent CreateCompletionEvent_NoMethodDeclarations()
        {
            var ce = CreateCompletionEvent();
            ce.Context2 = new Context();
            return ce;
        }

        private static IDEEvent CreateCompletionEvent_NoTriggerPoint()
        {
            var ce = CreateCompletionEvent();
            ce.Context2 = CreateContext(new ContinueStatement());
            return ce;
        }

        private static IDEEvent RemoveProposals(IDEEvent e)
        {
            var ince = e as CompletionEvent;
            Asserts.NotNull(ince);

            return new CompletionEvent
            {
                IDESessionUUID = e.IDESessionUUID,
                ActiveDocument = e.ActiveDocument,
                TriggeredAt = e.TriggeredAt,
                Context2 = ince.Context2
            };
        }
    }
}
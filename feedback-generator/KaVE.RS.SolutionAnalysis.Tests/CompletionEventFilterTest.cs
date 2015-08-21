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
            _sut = new CompletionEventFilter(@"C:\from\", @"C:\to\", _io, _logger);
        }


        [Test]
        public void HappyPath()
        {
            var e1 = CreateNonCompletionEvent();
            var e2 = CreateCompletionEvent();
            var e3 = CreateCompletionEvent_NoCSharpFile();
            var e4 = CreateCompletionEvent_Incomplete_NoSessionId();
            var e5 = CreateCompletionEvent_Incomplete_NoTriggerTime();
            var e6 = CreateCompletionEvent_NoMethodDeclarations();
            var e7 = CreateCompletionEvent_NoTriggerPoint();

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
                .Returns(e1)
                .Returns(e2)
                .Returns(e3)
                .Returns(e4)
                .Returns(e5)
                .Returns(e6)
                .Returns(e7);

            var output = new List<IDEEvent>();
            Mock.Get(_wa).Setup(wa => wa.Add(It.IsAny<IDEEvent>())).Callback<IDEEvent>(e => output.Add(e));

            _sut.Run();

            Mock.Get(_io).Verify(io => io.GetFilesRecursive(@"C:\from\", "*.zip"));
            Mock.Get(_io).Verify(io => io.ReadArchive(@"C:\from\a\a.zip"));
            Mock.Get(_io).Verify(io => io.CreateDirectory(@"C:\to\a"));
            Mock.Get(_io).Verify(io => io.CreateArchive(@"C:\to\a\a.zip"));

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual(e2, output[0]);
            Mock.Get(_wa).Verify(wa => wa.Dispose());

            Mock.Get(_logger).Verify(l => l.FoundZips(1));
            Mock.Get(_logger).Verify(l => l.ProgressZip(1, 1, @"C:\from\a\a.zip", @"C:\to\a\a.zip"));
            Mock.Get(_logger).Verify(l => l.FoundEvents(7));
            Mock.Get(_logger).Verify(l => l.ProgressEvent('.'), Times.Exactly(1));
            Mock.Get(_logger).Verify(l => l.ProgressEvent(':'), Times.Exactly(3));
            Mock.Get(_logger).Verify(l => l.ProgressEvent('|'), Times.Exactly(1));
            Mock.Get(_logger).Verify(l => l.ProgressEvent('o'), Times.Exactly(1));
            Mock.Get(_logger).Verify(l => l.ProgressEvent('x'), Times.Exactly(1));
            Mock.Get(_logger).Verify(l => l.Finish(7, 3, 1, 1, 1));
        }

        private IDEEvent CreateNonCompletionEvent()
        {
            var e = new CommandEvent();
            AddBasicInformation(e);
            return e;
        }


        private CompletionEvent CreateCompletionEvent()
        {
            var e = new CompletionEvent();
            AddBasicInformation(e);
            e.Context2 = CreateContext(
                new ContinueStatement(),
                new ExpressionStatement {Expression = new CompletionExpression()});
            return e;
        }

        private IDEEvent CreateCompletionEvent_NoCSharpFile()
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

        private IDEEvent CreateCompletionEvent_Incomplete_NoSessionId()
        {
            var ce = CreateCompletionEvent();
            ce.IDESessionUUID = null;
            return ce;
        }

        private IDEEvent CreateCompletionEvent_Incomplete_NoTriggerTime()
        {
            var ce = CreateCompletionEvent();
            ce.TriggeredAt = null;
            return ce;
        }

        private IDEEvent CreateCompletionEvent_NoMethodDeclarations()
        {
            var ce = CreateCompletionEvent();
            ce.Context2 = new Context();
            return ce;
        }

        private IDEEvent CreateCompletionEvent_NoTriggerPoint()
        {
            var ce = CreateCompletionEvent();
            ce.Context2 = CreateContext(new ContinueStatement());
            return ce;
        }
    }
}
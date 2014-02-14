using System;
using System.Collections.Generic;
using KaVE.CompletionTraceGenerator.Model;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names.CSharp;
using KaVE.VsFeedbackGenerator.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.CompletionTraceGenerator.Tests
{
    [TestFixture]
    public class CompletionEventToTraceConverterTest
    {
        private CompletionEventToTraceConverter _sut;
        private Mock<ILogWriter<CompletionTrace>> _writerMock;

        [SetUp]
        public void Setup()
        {
            _writerMock = new Mock<ILogWriter<CompletionTrace>>();

            _sut = new CompletionEventToTraceConverter(_writerMock.Object);
        }

        [Test]
        public void ShouldCreateTraceFromAppliedCompletion()
        {
            var @event = new CompletionEvent
            {
                TerminatedAs = CompletionEvent.TerminationState.Applied,
                TriggeredAt = DateTime.Now.AddMilliseconds(0),
                TerminatedAt = DateTime.Now.AddMilliseconds(234)
            };

            var expected = new CompletionTrace {DurationInMillis = 234};
            expected.AppendAction(CompletionAction.NewApply());

            _sut.Process(@event);

            _writerMock.Verify(writer => writer.Write(expected));
        }

        [Test]
        public void ShouldCreateTraceFromCancelledCompletion()
        {
            var triggeredAt = DateTime.Now;
            var @event = new CompletionEvent
            {
                TerminatedAs = CompletionEvent.TerminationState.Cancelled,
                TriggeredAt = triggeredAt,
                TerminatedAt = triggeredAt.AddMilliseconds(398)
            };

            var expected = new CompletionTrace {DurationInMillis = 398};
            expected.AppendAction(CompletionAction.NewCancel());

            _sut.Process(@event);

            _writerMock.Verify(writer => writer.Write(expected));
        }

        [Test]
        public void ShouldCreateTraceFromCompletionWithTwoStepActions()
        {
            var triggeredAt = DateTime.Now;
            var proposals = new List<Proposal>
            {
                CreateAnonymousProposal(),
                CreateAnonymousProposal(),
                CreateAnonymousProposal()
            };
            var @event = new CompletionEvent
            {
                TerminatedAs = CompletionEvent.TerminationState.Applied,
                TriggeredAt = triggeredAt,
                TerminatedAt = triggeredAt.AddMilliseconds(698),
                ProposalCollection = new ProposalCollection(proposals)
            };
            @event.AddSelection(proposals[0]);
            @event.AddSelection(proposals[1]);
            @event.AddSelection(proposals[2]);
            @event.AddSelection(proposals[1]);

            var expected = new CompletionTrace {DurationInMillis = 698};
            expected.AppendAction(CompletionAction.NewStep(Direction.Down));
            expected.AppendAction(CompletionAction.NewStep(Direction.Down));
            expected.AppendAction(CompletionAction.NewStep(Direction.Up));
            expected.AppendAction(CompletionAction.NewApply());

            _sut.Process(@event);

            _writerMock.Verify(writer => writer.Write(expected));
        }

        private static Proposal CreateAnonymousProposal()
        {
            return new Proposal {Name = Name.Get(Guid.NewGuid().ToString())};
        }

        //[Test]
        public void Asd()
        {
            var completionEvent = new CompletionEvent();
            _sut.Process(completionEvent);
            _sut.Process(completionEvent);
            _sut.Process(completionEvent);
            _sut.Process(completionEvent);

            var expected = new CompletionTrace();
            // ...

            _writerMock.Verify(writer => writer.Write(expected));
        }
    }
}
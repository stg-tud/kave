﻿using System;
using System.Collections.Generic;
using System.Linq;
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
            var @event = CreateAnonymousCompletionEvent(234);
            @event.TerminatedAs = CompletionEvent.TerminationState.Applied;

            var expected = new CompletionTrace {DurationInMillis = 234};
            expected.AppendAction(CompletionAction.NewApply());

            _sut.Process(@event);

            _writerMock.Verify(writer => writer.Write(expected));
        }

        [Test]
        public void ShouldCreateTraceFromCancelledCompletion()
        {
            var @event = CreateAnonymousCompletionEvent(398);
            @event.TerminatedAs = CompletionEvent.TerminationState.Cancelled;

            var expected = new CompletionTrace {DurationInMillis = 398};
            expected.AppendAction(CompletionAction.NewCancel());

            _sut.Process(@event);

            _writerMock.Verify(writer => writer.Write(expected));
        }

        [Test]
        public void ShouldCreateTraceFromCompletionWithTwoStepActions()
        {
            var @event = CreateAnonymousCompletionEvent(698);
            @event.TerminatedAs = CompletionEvent.TerminationState.Applied;
            var proposals = CreateAnonymousProposals(3);
            @event.ProposalCollection = new ProposalCollection(proposals);
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

        [Test]
        public void ShouldCreateTraceWithMouseGotoFromCompletionWithJump()
        {
            var @event = CreateAnonymousCompletionEvent(34);
            @event.TerminatedAs = CompletionEvent.TerminationState.Applied;
            var proposals = CreateAnonymousProposals(10);
            @event.ProposalCollection = new ProposalCollection(proposals);
            @event.AddSelection(proposals[0]);
            @event.AddSelection(proposals[9]);

            var expected = new CompletionTrace { DurationInMillis = 34 };
            expected.AppendAction(CompletionAction.NewMouseGoto(9));
            expected.AppendAction(CompletionAction.NewApply());

            _sut.Process(@event);

            _writerMock.Verify(writer => writer.Write(expected));
        }

        [Test]
        public void ShouldNotCreateTraceFromIncompleteCompletionWithFiltering()
        {
            var @event = CreateAnonymousCompletionEvent(66);
            @event.TerminatedAs = CompletionEvent.TerminationState.Filtered;

            _sut.Process(@event);

            _writerMock.Verify(writer => writer.Write(It.IsAny<CompletionTrace>()), Times.Never);
        }

        [Test]
        public void ShouldCreateTraceFromCompletionWithFiltering()
        {
            var firstEvent = CreateAnonymousCompletionEvent(33);
            firstEvent.TerminatedAs = CompletionEvent.TerminationState.Filtered;
            var secondEvent = CreateAnonymousCompletionEvent(42);
            secondEvent.TerminatedAs = CompletionEvent.TerminationState.Cancelled;
            secondEvent.Prefix = "Get";

            var expected = new CompletionTrace { DurationInMillis = 75 };
            expected.AppendAction(CompletionAction.NewFilter("Get"));
            expected.AppendAction(CompletionAction.NewCancel());

            _sut.Process(firstEvent);
            _sut.Process(secondEvent);

            _writerMock.Verify(writer => writer.Write(expected));
        }

        [Test]
        public void ShouldCreateTraceFromCompletionWithSteppingBeforeAndAfterFiltering()
        {
            var firstEvent = CreateAnonymousCompletionEvent(12);
            var proposals = CreateAnonymousProposals(6);
            firstEvent.ProposalCollection = new ProposalCollection(proposals);
            firstEvent.AddSelection(proposals[0]);
            firstEvent.AddSelection(proposals[1]);
            firstEvent.TerminatedAs = CompletionEvent.TerminationState.Filtered;
            var secondEvent = CreateAnonymousCompletionEvent(23);
            var filteredProposals = proposals.Take(4);
            secondEvent.Prefix = "isE";
            secondEvent.ProposalCollection = new ProposalCollection(filteredProposals);
            secondEvent.AddSelection(proposals[0]);
            secondEvent.AddSelection(proposals[3]);
            secondEvent.TerminatedAs = CompletionEvent.TerminationState.Applied;

            var expected = new CompletionTrace {DurationInMillis = 35};
            expected.AppendAction(CompletionAction.NewStep(Direction.Down));
            expected.AppendAction(CompletionAction.NewFilter("isE"));
            expected.AppendAction(CompletionAction.NewMouseGoto(3));
            expected.AppendAction(CompletionAction.NewApply());

            _sut.Process(firstEvent);
            _sut.Process(secondEvent);

            _writerMock.Verify(writer => writer.Write(expected));
        }

        [Test]
        public void ShouldCreateAsManyTracesAsTerminatedCompletionsAreProcessed()
        {
            var event1 = CreateAnonymousCompletionEvent(55);
            event1.TerminatedAs = CompletionEvent.TerminationState.Applied;
            var event2 = CreateAnonymousCompletionEvent(42);
            event2.TerminatedAs = CompletionEvent.TerminationState.Applied;
            var event3 = CreateAnonymousCompletionEvent(23);
            event3.TerminatedAs = CompletionEvent.TerminationState.Cancelled;
            var event4 = CreateAnonymousCompletionEvent(666);
            event4.TerminatedAs = CompletionEvent.TerminationState.Filtered;
            var event5 = CreateAnonymousCompletionEvent(99);
            event5.TerminatedAs = CompletionEvent.TerminationState.Applied;
            var event6 = CreateAnonymousCompletionEvent(69);
            event6.TerminatedAs = CompletionEvent.TerminationState.Cancelled;

            _sut.Process(event1);
            _sut.Process(event2);
            _sut.Process(event3);
            _sut.Process(event4);
            _sut.Process(event5);
            _sut.Process(event6);

            _writerMock.Verify(writer => writer.Write(It.IsAny<CompletionTrace>()), Times.Exactly(5));
        }

        [Test]
        public void ShouldIgnoreCompletionsIfEventStreamIsInconsistent()
        {
            // first event ends filtered, second starts fresh
        }

        private static CompletionEvent CreateAnonymousCompletionEvent(int duration)
        {
            var now = DateTime.Now;
            return new CompletionEvent {TriggeredAt = now, TerminatedAt = now.AddMilliseconds(duration)};
        }

        private static IList<Proposal> CreateAnonymousProposals(uint numberOfProposals)
        {
            var proposals = new List<Proposal>();
            for (var i = 0; i < numberOfProposals; i++)
            {
                proposals.Add(CreateAnonymousProposal());
            }
            return proposals;
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
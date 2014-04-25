using System.Linq;
using KaVE.CompletionTraceGenerator.Model;
using KaVE.Model.Events;
using KaVE.Model.Events.CompletionEvent;
using KaVE.VsFeedbackGenerator.Utils.Logging;
using Moq;
using NUnit.Framework;
using TestFactory = KaVE.TestUtils.Model.Events.CompletionEvent.CompletionEventTestFactory;

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
            var @event = TestFactory.CreateAnonymousCompletionEvent(5234);
            @event.TerminatedAs = CompletionEvent.TerminationState.Applied;

            var expected = new CompletionTrace {DurationInMillis = 5234};
            expected.AppendAction(CompletionAction.NewApply());

            _sut.Process(@event);

            _writerMock.Verify(writer => writer.Write(expected));
        }

        [Test]
        public void ShouldCreateTraceFromCancelledCompletion()
        {
            var @event = TestFactory.CreateAnonymousCompletionEvent(398);
            @event.TerminatedAs = CompletionEvent.TerminationState.Cancelled;

            var expected = new CompletionTrace {DurationInMillis = 398};
            expected.AppendAction(CompletionAction.NewCancel());

            _sut.Process(@event);

            _writerMock.Verify(writer => writer.Write(expected));
        }

        [Test]
        public void ShouldCreateTraceFromCompletionWithTwoStepActions()
        {
            var @event = TestFactory.CreateAnonymousCompletionEvent(698);
            @event.TerminatedAs = CompletionEvent.TerminationState.Applied;
            var proposals = TestFactory.CreateAnonymousProposals(3);
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
            var @event = TestFactory.CreateAnonymousCompletionEvent(34);
            @event.TerminatedAs = CompletionEvent.TerminationState.Applied;
            var proposals = TestFactory.CreateAnonymousProposals(10);
            @event.ProposalCollection = new ProposalCollection(proposals);
            @event.AddSelection(proposals[0]);
            @event.AddSelection(proposals[9]);

            var expected = new CompletionTrace {DurationInMillis = 34};
            expected.AppendAction(CompletionAction.NewMouseGoto(9));
            expected.AppendAction(CompletionAction.NewApply());

            _sut.Process(@event);

            _writerMock.Verify(writer => writer.Write(expected));
        }

        [Test]
        public void ShouldNotCreateTraceFromIncompleteCompletionWithFiltering()
        {
            var @event = TestFactory.CreateAnonymousCompletionEvent(66);
            @event.TerminatedAs = CompletionEvent.TerminationState.Filtered;

            _sut.Process(@event);

            _writerMock.Verify(writer => writer.Write(It.IsAny<CompletionTrace>()), Times.Never);
        }

        [Test]
        public void ShouldCreateTraceFromCompletionWithFiltering()
        {
            var firstEvent = TestFactory.CreateAnonymousCompletionEvent(33);
            firstEvent.TerminatedAs = CompletionEvent.TerminationState.Filtered;
            var secondEvent = TestFactory.CreateAnonymousCompletionEvent(42);
            secondEvent.TriggeredBy = IDEEvent.Trigger.Automatic;
            secondEvent.TerminatedAs = CompletionEvent.TerminationState.Cancelled;
            secondEvent.Prefix = "Get";

            var expected = new CompletionTrace {DurationInMillis = 75};
            expected.AppendAction(CompletionAction.NewFilter("Get"));
            expected.AppendAction(CompletionAction.NewCancel());

            _sut.Process(firstEvent);
            _sut.Process(secondEvent);

            _writerMock.Verify(writer => writer.Write(expected));
        }

        [Test]
        public void ShouldCreateTraceFromCompletionWithSteppingBeforeAndAfterFiltering()
        {
            var firstEvent = TestFactory.CreateAnonymousCompletionEvent(12);
            var proposals = TestFactory.CreateAnonymousProposals(6);
            firstEvent.ProposalCollection = new ProposalCollection(proposals);
            firstEvent.AddSelection(proposals[0]);
            firstEvent.AddSelection(proposals[1]);
            firstEvent.TerminatedAs = CompletionEvent.TerminationState.Filtered;
            var secondEvent = TestFactory.CreateAnonymousCompletionEvent(23);
            var filteredProposals = proposals.Take(4).ToList();
            secondEvent.TriggeredBy = IDEEvent.Trigger.Automatic;
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
            var event1 = TestFactory.CreateAnonymousCompletionEvent(55);
            event1.TerminatedAs = CompletionEvent.TerminationState.Applied;
            var event2 = TestFactory.CreateAnonymousCompletionEvent(42);
            event2.TerminatedAs = CompletionEvent.TerminationState.Applied;
            var event3 = TestFactory.CreateAnonymousCompletionEvent(23);
            event3.TerminatedAs = CompletionEvent.TerminationState.Cancelled;
            var event4 = TestFactory.CreateAnonymousCompletionEvent(666);
            event4.TerminatedAs = CompletionEvent.TerminationState.Filtered;
            var event5 = TestFactory.CreateAnonymousCompletionEvent(99);
            event5.TriggeredBy = IDEEvent.Trigger.Automatic;
            event5.TerminatedAs = CompletionEvent.TerminationState.Applied;
            var event6 = TestFactory.CreateAnonymousCompletionEvent(69);
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
        public void ShouldIgnoreInconsistentCompletions()
        {
            var firstEvent = TestFactory.CreateAnonymousCompletionEvent(33);
            firstEvent.TerminatedAs = CompletionEvent.TerminationState.Filtered;
            var secondEvent = TestFactory.CreateAnonymousCompletionEvent(42);
            secondEvent.TriggeredBy = IDEEvent.Trigger.Click;
            secondEvent.TerminatedAs = CompletionEvent.TerminationState.Applied;

            var expected = new CompletionTrace {DurationInMillis = 42};
            expected.AppendAction(CompletionAction.NewApply());

            _sut.Process(firstEvent);
            _sut.Process(secondEvent);

            _writerMock.Verify(writer => writer.Write(expected));
        }
    }
}
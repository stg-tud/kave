using System.Linq;
using KaVE.Model.Events.CompletionEvent;
using KaVE.VsFeedbackGenerator.Generators.Merging;
using KaVE.VsFeedbackGenerator.Tests.Utils;
using KaVE.VsFeedbackGenerator.Utils;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Generators.Merging
{
    [TestFixture]
    class CompletionEventMergingStrategyTest
    {
        private CompletionEventMergingStrategy _strategy;
        private CompletionEvent _firstFilterEvent;
        private CompletionEvent _secondFilterEvent;
        private CompletionEvent _initialEvent;
        private CompletionEvent _finalEvent;

        [SetUp]
        public void SetUpSubsequentCompletionEvents()
        {
            var proposals = ReSharperMockUtils.MockLookupItemList(5).ToProposalCollection();

            // first the user tried to find something manually (selection changes)
            _initialEvent = new CompletionEvent
            {
                Prefix = "",
                ProposalCollection = proposals,
                TerminatedAs = CompletionEvent.TerminationState.Filtered
            };
            _initialEvent.AddSelection(proposals.Proposals[4]);
            _initialEvent.AddSelection(proposals.Proposals[3]);

            // second, he started filtering by typing 'g'
            _firstFilterEvent = new CompletionEvent
            {
                Prefix = "g",
                ProposalCollection = proposals,
                TerminatedAs = CompletionEvent.TerminationState.Filtered,
            };
            _firstFilterEvent.AddSelection(proposals.Proposals[3]);

            // third, he immediately typed 'e' to narrow down the list further
            // thereby, the selection changed, because the previously selected proposal was filtered
            _secondFilterEvent = new CompletionEvent
            {
                Prefix = "ge",
                ProposalCollection = new ProposalCollection(_firstFilterEvent.ProposalCollection.Proposals.Take(3)),
                TerminatedAs = CompletionEvent.TerminationState.Filtered,
            };
            _secondFilterEvent.AddSelection(proposals.Proposals[0]);

            // last, he typed 't' and applied one of the remaining proposals.
            _finalEvent = new CompletionEvent
            {
                Prefix = "get",
                ProposalCollection = new ProposalCollection(_firstFilterEvent.ProposalCollection.Proposals.Take(2)),
                TerminatedAs = CompletionEvent.TerminationState.Applied,
            };
            _finalEvent.AddSelection(proposals.Proposals[0]);
            _finalEvent.AddSelection(proposals.Proposals[1]);
        }

        [SetUp]
        public void SetUpStrategyUnderTest()
        {
            _strategy = new CompletionEventMergingStrategy();
        }

        [Test]
        public void ShouldNotMergeIfEarlierEventContainsInteractions()
        {
            Assert.IsFalse(_strategy.Mergable(_initialEvent, _firstFilterEvent));
        }

        [Test]
        public void ShouldMergeSubsequentFilteringsWithSelectionChange()
        {
            Assert.IsTrue(_strategy.Mergable(_firstFilterEvent, _secondFilterEvent));
        }

        [Test]
        public void ShouldMergeSubsequentFilteringsWithoutSelectionChange()
        {
            Assert.IsTrue(_strategy.Mergable(_secondFilterEvent, _finalEvent));
        }
    }
}

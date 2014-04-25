using System;
using System.Collections.Generic;
using KaVE.Model.Events;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names.CSharp;

namespace KaVE.TestUtils.Model.Events.CompletionEvent
{
    public static class CompletionEventTestFactory
    {
        public static KaVE.Model.Events.CompletionEvent.CompletionEvent CreateAnonymousCompletionEvent(int duration)
        {
            var now = DateTime.Now;
            return new KaVE.Model.Events.CompletionEvent.CompletionEvent
            {
                TriggeredAt = now,
                TriggeredBy = IDEEvent.Trigger.Shortcut,
                TerminatedAt = now.AddTicks(duration * TimeSpan.TicksPerMillisecond)
            };
        }

        public static IList<Proposal> CreateAnonymousProposals(uint numberOfProposals)
        {
            var proposals = new List<Proposal>();
            for (var i = 0; i < numberOfProposals; i++)
            {
                proposals.Add(CreateAnonymousProposal());
            }
            return proposals;
        }

        public static Proposal CreateAnonymousProposal()
        {
            return new Proposal { Name = Name.Get(Guid.NewGuid().ToString()) };
        }
    }
}

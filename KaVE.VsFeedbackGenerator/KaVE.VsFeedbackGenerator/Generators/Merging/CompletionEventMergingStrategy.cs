using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Util;
using KaVE.Model.Events;
using KaVE.Model.Events.CompletionEvent;

namespace KaVE.VsFeedbackGenerator.Generators.Merging
{
    internal class CompletionEventMergingStrategy : IEventMergeStrategy
    {
        public bool Mergable(IDEEvent @event, IDEEvent subsequentEvent)
        {
            var ce1 = @event as CompletionEvent;
            if (ce1 == null) return false;
            var ce2 = subsequentEvent as CompletionEvent;
            if (ce2 == null) return false;
            return Mergable(ce1, ce2);
        }

        private bool Mergable(CompletionEvent @event, CompletionEvent subsequentEvent)
        {
            return (@event.TerminatedAs == CompletionEvent.TerminationState.Filtered) &&
                   (@event.Selections.Count <= 1);
        }

        public IDEEvent Merge(IDEEvent @event, IDEEvent subsequentEvent)
        {
            throw new NotImplementedException();
        }
    }
}

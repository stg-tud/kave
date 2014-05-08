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
using KaVE.Model.Events;
using KaVE.Model.Events.CompletionEvent;

namespace KaVE.VsFeedbackGenerator.Generators.Merging
{
    /// <summary>
    /// Merges intermediate filtering events with the subsequent completion event. An intermediate filtering event, is
    /// a completion event that was triggered automatically (due to a filtering), recorded no interactions of the user,
    /// and was terminated by another filtering. Such event are fired, when multiple characters are typed directly
    /// after oneanother. For example, if the user types 'get', three events are fired for 'g', 'e', and 't'. The
    /// strategy merges them down to one event.
    /// </summary>
    /// TODO strategy currently erases information: when the user types something and then deletes (part of) it, only the final prefix is kept.
    internal class CompletionEventMergingStrategy : IEventMergeStrategy
    {
        public bool AreMergable(IDEEvent @event, IDEEvent subsequentEvent)
        {
            var ce1 = @event as CompletionEvent;
            if (ce1 == null)
            {
                return false;
            }
            if (!(subsequentEvent is CompletionEvent))
            {
                return false;
            }
            return IsMergable(ce1);
        }

        private static bool IsMergable(CompletionEvent @event)
        {
            var eventIsIntermediateFilterEvent = (@event.TriggeredBy == IDEEvent.Trigger.Automatic) &&
                    (@event.TerminatedAs == CompletionEvent.TerminationState.Filtered);
            // If there is no selection, we know that there was no interaction. If there is one selection, it may be
            // the previous selection or an initial selection (if there was no selection before). We cannot distinguish
            // this here. However, since we never merge the initial event (triggeredBy != Automatic), we know whether
            // there was an inital selection or not. Therefore, we can deduce that a selection was made during or after
            // the filtering.
            var eventContainsNoInteractions = (@event.Selections.Count <= 1);
            return eventIsIntermediateFilterEvent && eventContainsNoInteractions;
        }

        public IDEEvent Merge(IDEEvent @event, IDEEvent subsequentEvent)
        {
            var evt1 = (CompletionEvent) @event;
            var evt2 = (CompletionEvent) subsequentEvent;

            return new CompletionEvent
            {
                // properties that could be taken from both events (as their values are equal)
                IDESessionUUID = evt2.IDESessionUUID,
                ActiveDocument = evt2.ActiveDocument,
                ActiveWindow = evt2.ActiveWindow,
                Context = evt2.Context,
                // properties that need be taken from first event
                TriggeredBy = evt1.TriggeredBy,
                TriggeredAt = evt1.TriggeredAt,
                // properties that need be taken from later event
                Prefix = evt2.Prefix,
                ProposalCollection = evt2.ProposalCollection,
                Selections = evt2.Selections,
                TerminatedAs = evt2.TerminatedAs,
                TerminatedAt = evt2.TerminatedAt,
                TerminatedBy = evt2.TerminatedBy,
            };
        }
    }
}
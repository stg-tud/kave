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
 * 
 * Contributors:
 *    - Markus Zimmermann
 */

using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Cleanup.Processors
{
    internal class EquivalentCommandProcessor : BaseProcessor
    {
        public EquivalentCommandProcessor()
        {
            RegisterFor<ConcurrentEvent>(ProcessConcurrentEvent);
        }

        private IKaVESet<IDEEvent> ProcessConcurrentEvent(ConcurrentEvent @event)
        {
            return HasEquivalentCommands(@event) ? AnswerKeep() : AnswerDrop();
        }

        private bool HasEquivalentCommands(ConcurrentEvent concurrentEvent)
        {
            var concurrentEventList = concurrentEvent.ConcurrentEventList;
            if (!ContainsOnlyCommandEvents(concurrentEventList)) return false;
            var commandEventList = concurrentEventList.Cast<CommandEvent>().ToList();
            return ContainsEquivalentCommands(commandEventList);
        }

        private static bool ContainsEquivalentCommands(IList<CommandEvent> commandEventList)
        {
            if (commandEventList.Count() != 2) return false;
            var triggerOfCommandEvent1 = commandEventList[0].TriggeredBy;
            var triggerOfCommandEvent2 = commandEventList[1].TriggeredBy;
            return HaveDifferentTrigger(triggerOfCommandEvent1, triggerOfCommandEvent2);
        }

        private static bool HaveDifferentTrigger(IDEEvent.Trigger triggerOfCommandEvent1, IDEEvent.Trigger triggerOfCommandEvent2)
        {
            return triggerOfCommandEvent1 != triggerOfCommandEvent2;
        }

        private static bool ContainsOnlyCommandEvents(IEnumerable<IDEEvent> concurrentEventList)
        {
            return concurrentEventList.All(ideEvent => ideEvent is CommandEvent);
        }
    }
}

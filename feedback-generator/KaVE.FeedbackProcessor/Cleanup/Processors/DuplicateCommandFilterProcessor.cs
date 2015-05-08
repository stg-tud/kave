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
 *    - Mattis Manfred Kämmerer
 */

using System;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Cleanup.Heuristics;

namespace KaVE.FeedbackProcessor.Cleanup.Processors
{
    internal class DuplicateCommandFilterProcessor : BaseProcessor
    {
        public static TimeSpan CommandEventTimeDifference = new TimeSpan(0, 0, 0, 0, 10);

        private CommandEvent _oldCommandEvent;

        public DuplicateCommandFilterProcessor()
        {
            RegisterFor<CommandEvent>(FilterCommandEvents);
        }

        public IKaVESet<IDEEvent> FilterCommandEvents(CommandEvent commandEvent)
        {
            if (IsDuplicate(commandEvent))
            {
                return AnswerDrop();
            }

            _oldCommandEvent = commandEvent;

            return AnswerKeep();
        }

        private bool IsDuplicate(CommandEvent commandEvent)
        {
            return _oldCommandEvent != null && _oldCommandEvent.CommandId == commandEvent.CommandId &&
                   ConcurrentEventHeuristic.HaveSimiliarEventTime(
                       ConcurrentEventHeuristic.GetValidEventTime(_oldCommandEvent.TriggeredAt),
                       ConcurrentEventHeuristic.GetValidEventTime(commandEvent.TriggeredAt), CommandEventTimeDifference);
        }
    }
}
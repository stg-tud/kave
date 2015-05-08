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
 *    - Markus Zimmermann
 */

using System;
using System.Collections.Generic;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Cleanup.Heuristics;

namespace KaVE.FeedbackProcessor.Cleanup.Processors
{
    class MergeEquivalentCommands : BaseProcessor
    {
        public readonly IList<SortedSet<string>> Statistic = new List<SortedSet<string>>();

        public static TimeSpan EventTimeDifference = new TimeSpan(0,0,0,0,10);

        private CommandEvent _lastCommandEvent;

        private CommandEvent _mergedCommandEvent; 

        public MergeEquivalentCommands()
        {
            RegisterFor<CommandEvent>(HandleCommandEvent);
        }

        private IKaVESet<IDEEvent> HandleCommandEvent(CommandEvent commandEvent)
        {
            if (_lastCommandEvent == null)
            {
                _lastCommandEvent = commandEvent;
                _mergedCommandEvent = commandEvent;
                return AnswerDrop();
            }
            
            if(ConcurrentEventHeuristic.HaveSimiliarEventTime(
                    _lastCommandEvent.TriggeredAt,
                    commandEvent.TriggeredAt,
                    EventTimeDifference))
            {
                AddEquivalentCommandsToStatistic(commandEvent);

                _mergedCommandEvent = MergeCommandHeuristic.MergeCommandEvents(_mergedCommandEvent, commandEvent);

                _lastCommandEvent = commandEvent;

                return AnswerDrop();
            }

            var resultEvent = _mergedCommandEvent;

            _lastCommandEvent = commandEvent;
            _mergedCommandEvent = commandEvent;
            
            return AnswerReplace(resultEvent);
        }

        private void AddEquivalentCommandsToStatistic(CommandEvent commandEvent)
        {
            var resultSet = new SortedSet<string>
            {
                _lastCommandEvent.CommandId,
                commandEvent.CommandId
            };

            if (!Statistic.Contains(resultSet))
            {
                Statistic.Add(resultSet);
            }
        }
    }
}

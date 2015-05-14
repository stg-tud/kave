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
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Cleanup.Heuristics;
using KaVE.FeedbackProcessor.Utils;

namespace KaVE.FeedbackProcessor.Cleanup.Processors
{
    internal class MapEquivalentCommandsProcessor : BaseProcessor
    {
        public static List<SortedCommandPair> Mappings = new List<SortedCommandPair>();

        public static readonly TimeSpan EventTimeDifference = TimeSpan.FromMilliseconds(10);

        private CommandEvent _unmappedCommandEvent;

        public MapEquivalentCommandsProcessor()
        {
            RegisterFor<CommandEvent>(MapCommandEvent);
        }

        private IKaVESet<IDEEvent> MapCommandEvent(CommandEvent commandEvent)
        {
            var resultSet = Sets.NewHashSet<IDEEvent>();
            resultSet.Add(commandEvent);

            var mapping = FindMappingFromLeftSideFor(commandEvent);
            var isOnLeftSide = mapping != null;
            if (isOnLeftSide)
            {
                if (_unmappedCommandEvent != null && _unmappedCommandEvent.CommandId.Equals(mapping.Item2))
                {
                    _unmappedCommandEvent = null;
                }

                resultSet.Remove(commandEvent);
                resultSet.Add(CopyCommandEventWithId(commandEvent, mapping.Item2));
            }

            if (_unmappedCommandEvent != null && IsLate(commandEvent))
            {
                resultSet.Add(_unmappedCommandEvent);
                _unmappedCommandEvent = null;
            }

            mapping = FindMappingFromRightSideFor(commandEvent);
            var isOnRightSide = mapping != null;
            if (isOnRightSide)
            {
                resultSet.Remove(commandEvent);
                _unmappedCommandEvent = commandEvent;
            }

            return resultSet;
        }

        private static CommandEvent CopyCommandEventWithId(IDEEvent commandEvent, string newId)
        {
            return new CommandEvent
            {
                CommandId = newId,
                ActiveDocument = commandEvent.ActiveDocument,
                ActiveWindow = commandEvent.ActiveWindow,
                IDESessionUUID = commandEvent.IDESessionUUID,
                KaVEVersion = commandEvent.KaVEVersion,
                TriggeredAt = commandEvent.TriggeredAt,
                TerminatedAt = commandEvent.TerminatedAt,
                TriggeredBy = commandEvent.TriggeredBy
            };
        }

        private bool IsLate(IDEEvent commandEvent)
        {
            return !ConcurrentEventHeuristic.HaveSimiliarEventTime(
                commandEvent.TriggeredAt,
                _unmappedCommandEvent.TriggeredAt,
                EventTimeDifference);
        }

        private static SortedCommandPair FindMappingFromLeftSideFor(CommandEvent commandEvent)
        {
            return Mappings.Find(pair => (pair.Item1.Equals(commandEvent.CommandId)));
        }

        private static SortedCommandPair FindMappingFromRightSideFor(CommandEvent commandEvent)
        {
            return Mappings.Find(pair => (pair.Item2.Equals(commandEvent.CommandId)));
        }
    }
}
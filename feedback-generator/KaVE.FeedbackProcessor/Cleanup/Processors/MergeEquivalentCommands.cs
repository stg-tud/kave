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

namespace KaVE.FeedbackProcessor.Cleanup.Processors
{
    internal class MergeEquivalentCommands : BaseProcessor
    {
        public static ISet<ISet<string>> Statistic = new KaVEHashSet<ISet<string>>();

        public static TimeSpan EventTimeDifference = new TimeSpan(0, 0, 0, 0, 10);

        public static readonly List<CommandEvent> SingleClickEventCache = new List<CommandEvent>();

        private CommandEvent _lastCommandEvent;

        private CommandEvent _mergedCommandEvent;

        public MergeEquivalentCommands()
        {
            RegisterFor<CommandEvent>(HandleCommandEvent);
        }

        private IKaVESet<IDEEvent> HandleCommandEvent(CommandEvent commandEvent)
        {
//            if (IsClickEvent(commandEvent))
//            {
//                SingleClickEventCache.Add(commandEvent);
//            }

            var lastCommandEvent = _lastCommandEvent;
            _lastCommandEvent = commandEvent;

            var resultSet = Sets.NewHashSet<IDEEvent>();

            if (lastCommandEvent != null && ConcurrentEventHeuristic.HaveSimiliarEventTime(
                lastCommandEvent.TriggeredAt,
                commandEvent.TriggeredAt,
                EventTimeDifference))
            {
                AddEquivalentCommandsToStatistic(lastCommandEvent, commandEvent);

                _mergedCommandEvent = MergeCommandHeuristic.MergeCommandEvents(_mergedCommandEvent, commandEvent);

//                if ((IsClickEvent(_mergedCommandEvent)) && SingleClickEventCache.Count > 0)
//                {
//                    SingleClickEventCache.Remove(SingleClickEventCache.Last());
//                }
            }
            else
            {                
                if (_mergedCommandEvent != null)
                {
                    resultSet.Add(_mergedCommandEvent);
                }

                _mergedCommandEvent = commandEvent;
            }

            return resultSet;
        }

        private IKaVESet<IDEEvent> GetReplaceableClickEventsFromCache()
        {
            var resultSet = Sets.NewHashSet<IDEEvent>();
            if (Statistic.Count == 0)
            {
                return resultSet;
            }
            var cacheCopy = new List<CommandEvent>(SingleClickEventCache);
            foreach (var clickEvent in cacheCopy)
            {
                var clickEventCopy = clickEvent;
                foreach (var commandTuple in Statistic.Select(set => set.ToList())
                                                      .Where(commandTuple => commandTuple.Count == 2)
                                                      .Where(
                                                          commandTuple =>
                                                              commandTuple.Any( commandId =>
                                                                  commandId.Equals(clickEventCopy.CommandId)) &&
                                                              commandTuple.Any(MergeCommandHeuristic.IsVisualStudioCommandId)))
                {
                    resultSet.Add(clickEvent.CommandId.Equals(commandTuple[0])
                        ? CreateCopyWithDifferentCommandId(clickEvent,commandTuple[1])
                        : CreateCopyWithDifferentCommandId(clickEvent,commandTuple[0]));
                    SingleClickEventCache.Remove(clickEvent);
                    break;
                }
            }
            return resultSet;
        }

        private static bool IsClickEvent(CommandEvent commandEvent)
        {
            return commandEvent.TriggeredBy == IDEEvent.Trigger.Click;
        }

        private static void AddEquivalentCommandsToStatistic(CommandEvent lastCommandEvent, CommandEvent commandEvent)
        {
            var resultSet = Sets.NewHashSet(lastCommandEvent.CommandId, commandEvent.CommandId);

            Statistic.Add(resultSet);
        }

        private static CommandEvent CreateCopyWithDifferentCommandId(CommandEvent commandEvent, string commandId)
        {
            return new CommandEvent
            {
                ActiveDocument = commandEvent.ActiveDocument,
                ActiveWindow = commandEvent.ActiveWindow,
                CommandId = commandId,
                IDESessionUUID = commandEvent.IDESessionUUID,
                KaVEVersion = commandEvent.KaVEVersion,
                TriggeredAt = commandEvent.TriggeredAt,
                TerminatedAt = commandEvent.TerminatedAt,
                TriggeredBy = commandEvent.TriggeredBy
            };
        }
    }
}
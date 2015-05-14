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
using System.Text.RegularExpressions;
using KaVE.Commons.Model.Events;

namespace KaVE.FeedbackProcessor.Cleanup.Heuristics
{
    internal class MergeCommandHeuristic
    {
        public static CommandEvent MergeCommandEvents(CommandEvent commandEvent1, CommandEvent commandEvent2)
        {
            var visualStudioEvent = IsVisualStudioCommandEvent(commandEvent1) ? commandEvent1 : commandEvent2;

            return new CommandEvent
            {
                ActiveDocument = visualStudioEvent.ActiveDocument,
                ActiveWindow = visualStudioEvent.ActiveWindow,
                CommandId = visualStudioEvent.CommandId,
                IDESessionUUID = visualStudioEvent.IDESessionUUID,
                KaVEVersion = visualStudioEvent.KaVEVersion,
                TriggeredAt = GetEarliestTriggeredAt(commandEvent1, commandEvent2),
                TerminatedAt = GetLatestTerminatedAt(commandEvent1, commandEvent2),
                TriggeredBy = GetMergedTriggeredBy(commandEvent1, commandEvent2)
            };
        }

        private static DateTime? GetEarliestTriggeredAt(CommandEvent commandEvent1, CommandEvent commandEvent2)
        {
            return commandEvent1.TriggeredAt < commandEvent2.TriggeredAt
                ? commandEvent1.TriggeredAt
                : commandEvent2.TriggeredAt;
        }

        private static DateTime? GetLatestTerminatedAt(CommandEvent commandEvent1, CommandEvent commandEvent2)
        {
            return commandEvent1.TerminatedAt > commandEvent2.TerminatedAt
                ? commandEvent1.TerminatedAt
                : commandEvent2.TerminatedAt;
        }

        private static IDEEvent.Trigger GetMergedTriggeredBy(CommandEvent commandEvent1, CommandEvent commandEvent2)
        {
            return commandEvent1.TriggeredBy != IDEEvent.Trigger.Unknown
                ? commandEvent1.TriggeredBy
                : commandEvent2.TriggeredBy;
        }

        private static bool IsVisualStudioCommandEvent(CommandEvent commandEvent)
        {
            return commandEvent.CommandId != null && IsVisualStudioCommandId(commandEvent.CommandId);
        }

        public static bool IsVisualStudioCommandId(string commandId)
        {
            return new Regex(@"^\{.*\}:.*:").IsMatch(commandId);
        }

        public static bool IsReSharperCommandId(string commandId)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
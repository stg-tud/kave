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

using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;

namespace KaVE.VS.Statistics.Filters
{
    /// <summary>
    ///     Filter, merge and replacement logic for CommandEvents
    /// </summary>
    public class CommandPreprocessor : IEventPreprocessor
    {
        public IDEEvent Preprocess(IDEEvent @event)
        {
            var documentEvent = @event as DocumentEvent;
            if (documentEvent != null)
            {
                return Process(documentEvent);
            }

            var commandEvent = @event as CommandEvent;
            return commandEvent == null ? null : Process(commandEvent);
        }

        /// <summary>
        ///     Replaces Closing DocumentEvents by a new CommandEvent with CommandID "Close";
        ///     Returns null for each other DocumentEvent
        /// </summary>
        private static IDEEvent Process(DocumentEvent documentEvent)
        {
            if (documentEvent.Action != DocumentEvent.DocumentAction.Closing)
            {
                return null;
            }

            var commandEvent = new CommandEvent
            {
                ActiveDocument = documentEvent.ActiveDocument,
                ActiveWindow = documentEvent.ActiveWindow,
                CommandId = "Close",
                Duration = documentEvent.Duration,
                IDESessionUUID = documentEvent.IDESessionUUID,
                TerminatedAt = documentEvent.TerminatedAt,
                TriggeredAt = documentEvent.TriggeredAt,
                TriggeredBy = documentEvent.TriggeredBy
            };

            return commandEvent;
        }

        /// <summary>
        ///     Returns the Event if it is not filtered or null if it is filtered
        /// </summary>
        private static IDEEvent Process(CommandEvent commandEvent)
        {
            return IsFilteredCommandEvent(commandEvent) ? null : commandEvent;
        }

        /// <summary>
        ///     Implements Filter logic for CommandEvents;
        ///     <para>Returns true iff at least one of the following Conditions is met: </para>
        ///     <para>- <paramref name="commandEvent" /> has no CommandID</para>
        ///     <para>- <paramref name="commandEvent" /> is no Commit Event AND triggered by a click</para>
        ///     <para>- <paramref name="commandEvent" /> is a Close Event (DocumentEvent replacement is used for tracking closings)</para>
        /// </summary>
        private static bool IsFilteredCommandEvent(CommandEvent commandEvent)
        {
            if (commandEvent.CommandId == null)
            {
                return true;
            }

            if (commandEvent.CommandId == "Comm_it")
            {
                return false;
            }

            if (commandEvent.TriggeredBy == IDEEvent.Trigger.Click)
            {
                return true;
            }

            var index = commandEvent.CommandId.LastIndexOf(':');
            var commandType = index == -1 ? commandEvent.CommandId : commandEvent.CommandId.Substring(index + 1);
            switch (commandType)
            {
                case "File.Close":
                    return true;
                case "Close":
                    return true;
                default:
                    return false;
            }
        }
    }
}
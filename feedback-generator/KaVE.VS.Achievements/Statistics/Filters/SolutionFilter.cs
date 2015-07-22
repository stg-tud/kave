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
using KaVE.Commons.Model.Names.VisualStudio;

namespace KaVE.VS.Achievements.Statistics.Filters
{
    /// <summary>
    ///     Filter, merge and replacement logic for SolutionEvents
    ///     (Currently only implements the workaround/replacement for AddProjectItem SolutionEvents)
    /// </summary>
    public class SolutionFilter : IEventFilter
    {
        // -------------------------------
        // Workaround for TestClassesAdded
        // -------------------------------

        private string _newItemFileName;

        public IDEEvent Process(IDEEvent @event)
        {
            var commandEvent = @event as CommandEvent;
            if (commandEvent != null)
            {
                return Process(commandEvent);
            }

            var documentEvent = @event as DocumentEvent;
            if (documentEvent != null)
            {
                return Process(documentEvent);
            }

            var solutionEvent = @event as SolutionEvent;
            return solutionEvent;
        }

        /// <summary>
        ///     Replaces CommandEvents occuring when a new item is added
        ///     with a new SolutionEvent with the AddProjectItem action
        /// </summary>
        private IDEEvent Process(CommandEvent commandEvent)
        {
            if (!IsAddNewItemCommand(commandEvent))
            {
                return null;
            }
            var solutionEvent = new SolutionEvent
            {
                Action = SolutionEvent.SolutionAction.AddProjectItem,
                ActiveDocument = commandEvent.ActiveDocument,
                ActiveWindow = commandEvent.ActiveWindow,
                Duration = commandEvent.Duration,
                IDESessionUUID = commandEvent.IDESessionUUID,
                TerminatedAt = commandEvent.TerminatedAt,
                TriggeredAt = commandEvent.TriggeredAt,
                TriggeredBy = commandEvent.TriggeredBy,
                Target = new ComponentName(_newItemFileName)
            };
            _newItemFileName = "";
            return solutionEvent;
        }

        private static bool IsAddNewItemCommand(CommandEvent commandEvent)
        {
            return commandEvent.CommandId != null &&
                   (commandEvent.CommandId.Contains("AddNewItem") ||
                    commandEvent.CommandId.Contains("Project.AddClass"));
        }

        /// <summary>
        ///     Saves the FileName of <paramref name="documentEvent" />
        ///     for replacing the next CommandEvent;
        ///     <para>Returns null (filters all DocumentEvents)</para>
        /// </summary>
        private IDEEvent Process(DocumentEvent documentEvent)
        {
            _newItemFileName = documentEvent.Document == null || documentEvent.Document.FileName == null
                ? ""
                : documentEvent.Document.FileName;

            return null;
        }

        /// <summary>
        ///     Used as a Target value for the SolutionEvent created in the replacement logic
        /// </summary>
        public class ComponentName : IIDEComponentName
        {
            public ComponentName(string identifier)
            {
                Identifier = identifier;
            }

            public string Identifier { get; private set; }

            public bool IsUnknown
            {
                get { return false; }
            }
        }
    }
}
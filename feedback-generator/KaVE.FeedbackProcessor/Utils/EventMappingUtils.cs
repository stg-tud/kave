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
 *    - Sven Amann
 *    - Mattis Manfred Kämmerer
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Events.VisualStudio;

namespace KaVE.FeedbackProcessor.Utils
{
    internal static class EventMappingUtils
    {
        private const string Separator = " -> ";

        private static readonly IDictionary<Type, Func<IDEEvent, string>> ToStringMappings =
            new Dictionary<Type, Func<IDEEvent, string>>
            {
                {
                    typeof (CommandEvent),
                    (commandEvent => FormatString("Command", ((CommandEvent) commandEvent).CommandId))
                },
                {
                    typeof (WindowEvent),
                    (windowEvent => FormatString("Window", ((WindowEvent) windowEvent).Action.ToString()))
                },
                {
                    typeof (DocumentEvent),
                    (documentEvent => FormatString("Document", ((DocumentEvent) documentEvent).Action.ToString()))
                },
                {
                    typeof (BuildEvent),
                    (buildEvent => FormatString("Build", ((BuildEvent) buildEvent).Action))
                },
                {
                    typeof (EditEvent),
                    (editEvent =>
                        FormatString(
                            "Edit",
                            ((EditEvent) editEvent).NumberOfChanges.ToString(CultureInfo.InvariantCulture) + " Changes"))
                },
                {
                    typeof (DebuggerEvent),
                    (debuggerEvent => FormatString("Debugger", ((DebuggerEvent) debuggerEvent).Reason))
                },
                {
                    typeof (IDEStateEvent),
                    (ideStateEvent =>
                        FormatString("IDEState", ((IDEStateEvent) ideStateEvent).IDELifecyclePhase.ToString()))
                },
                {
                    typeof (SolutionEvent),
                    (solutionEvent => FormatString("Solution", ((SolutionEvent) solutionEvent).Action.ToString()))
                },
                {
                    typeof (CompletionEvent),
                    (completionEvent =>
                        FormatString(
                            "Completion",
                            ("Terminated as " + ((CompletionEvent) completionEvent).TerminatedState.ToString())))
                },
                {
                    typeof (ErrorEvent),
                    (errorEvent =>
                    {
                        var stackTraceString = ((ErrorEvent) errorEvent).StackTrace.First();
                        var index = stackTraceString.IndexOf(':');
                        return FormatString("Error", stackTraceString.Substring(0, index));
                    })
                }
            };

        public static void CopyIDEEventPropertiesFrom(this IDEEvent target, IDEEvent source)
        {
            target.IDESessionUUID = source.IDESessionUUID;
            target.KaVEVersion = source.KaVEVersion;
            target.TriggeredAt = source.TriggeredAt;
            target.TriggeredBy = source.TriggeredBy;
            target.Duration = source.Duration;
            target.ActiveWindow = source.ActiveWindow;
            target.ActiveDocument = source.ActiveDocument;
        }

        public static string GetAbstractStringOf(IDEEvent @event)
        {
            Func<IDEEvent, string> mapToString;
            return ToStringMappings.TryGetValue(@event.GetType(), out mapToString)
                ? mapToString(@event)
                : FormatString(@event.GetType().ToString(), "no mapping found");
        }

        private static string FormatString(string prefix, string suffix)
        {
            return String.Format("{0}{1}{2}", prefix, Separator, suffix);
        }
    }
}
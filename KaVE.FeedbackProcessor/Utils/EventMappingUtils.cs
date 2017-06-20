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

using System;
using System.Collections.Generic;
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
                    (commandEvent => ((CommandEvent) commandEvent).CommandId)
                },
                {
                    typeof (WindowEvent),
                    (windowEvent => ((WindowEvent) windowEvent).Action.ToString())
                },
                {
                    typeof (DocumentEvent),
                    (documentEvent => ((DocumentEvent) documentEvent).Action.ToString())
                },
                {
                    typeof (BuildEvent),
                    (buildEvent => ((BuildEvent) buildEvent).Action)
                },
                {
                    typeof (EditEvent),
                    (editEvent => "")
                },
                {
                    typeof (DebuggerEvent),
                    (debuggerEvent => ((DebuggerEvent) debuggerEvent).Reason)
                },
                {
                    typeof (IDEStateEvent),
                    (ideStateEvent => ((IDEStateEvent) ideStateEvent).IDELifecyclePhase.ToString())
                },
                {
                    typeof (SolutionEvent),
                    (solutionEvent => ((SolutionEvent) solutionEvent).Action.ToString())
                },
                {
                    typeof (CompletionEvent),
                    (completionEvent =>
                        ("Terminated as " + ((CompletionEvent) completionEvent).TerminatedState.ToString()))
                },
                {
                    typeof (ErrorEvent),
                    (errorEvent =>
                        ((ErrorEvent) errorEvent).StackTrace.First()
                                                 .Substring(
                                                     0,
                                                     ((ErrorEvent) errorEvent).StackTrace.First().IndexOf(':')))
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
                ? FormatString(@event.GetType().Name, mapToString(@event))
                : FormatString(@event.GetType().Name, "no mapping found");
        }

        private static string FormatString(string prefix, string suffix)
        {
            return String.Format("{0}{1}{2}", prefix, Separator, suffix);
        }
    }
}
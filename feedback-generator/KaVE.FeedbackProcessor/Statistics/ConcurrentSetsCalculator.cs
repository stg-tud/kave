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
using System.Globalization;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Statistics
{
    internal class ConcurrentSetsCalculator : IEventProcessor
    {
        private const string Separator = " -> ";

        public readonly IDictionary<ISet<string>, int> Statistic =
            new Dictionary<ISet<string>, int>();

        private static readonly IDictionary<Type, Func<IDEEvent, string>> ToStringMappings = 
            new Dictionary<Type, Func<IDEEvent, string>>
        {
            {
                typeof (CommandEvent), 
                (commandEvent => FormatString("Command", ((CommandEvent) commandEvent).CommandId))},
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
                    FormatString("Edit", ((EditEvent) editEvent).NumberOfChanges.ToString(CultureInfo.InvariantCulture) + " Changes"))
            },
            {
                typeof (DebuggerEvent), 
                (debuggerEvent => FormatString("Debugger", ((DebuggerEvent) debuggerEvent).Reason))
            },
            {
                typeof (IDEStateEvent),
                (ideStateEvent => FormatString("IDEState", ((IDEStateEvent) ideStateEvent).IDELifecyclePhase.ToString()))
            },
            {
                typeof (SolutionEvent),
                (solutionEvent => FormatString("Solution", ((SolutionEvent) solutionEvent).Action.ToString()))
            },
            {
                typeof (CompletionEvent),
                (completionEvent =>
                    FormatString("Completion", ("Terminated as " + ((CompletionEvent) completionEvent).TerminatedState.ToString())))
            }
        };

        public void OnEvent(IDEEvent @event)
        {
            var concurrentEvent = @event as ConcurrentEvent;
            if (concurrentEvent == null)
            {
                return;
            }

            var resultSet = Sets.NewHashSet<string>();

            foreach (var ideEvent in concurrentEvent.ConcurrentEventList)
            {
                Func<IDEEvent, string> mapToString;
                ToStringMappings.TryGetValue(ideEvent.GetType(), out mapToString);

                if (mapToString != null)
                {
                    resultSet.Add(mapToString(ideEvent));
                }
            }

            if (Statistic.ContainsKey(resultSet))
            {
                Statistic[resultSet]++;
            }
            else
            {
                if(resultSet.Count > 0) Statistic.Add(resultSet, 1);
            }
        }

        private static string FormatString(string prefix, string suffix)
        {
            return string.Format("{0}{1}{2}", prefix, Separator, suffix);
        }

        public void OnStreamStarts(Developer value) {}

        public void OnStreamEnds() {}
    }
}
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

using System.Collections.Generic;
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
                if (ideEvent is CommandEvent)
                {
                    resultSet.Add(MapToString((CommandEvent) ideEvent));
                }
                if (ideEvent is WindowEvent)
                {
                    resultSet.Add(MapToString((WindowEvent) ideEvent));
                }
                if (ideEvent is DocumentEvent)
                {
                    resultSet.Add(MapToString((DocumentEvent) ideEvent));
                }
                if (ideEvent is BuildEvent)
                {
                    resultSet.Add(MapToString((BuildEvent) ideEvent));
                }
                if (ideEvent is EditEvent)
                {
                    resultSet.Add(MapToString((EditEvent) ideEvent));
                }
                if (ideEvent is DebuggerEvent)
                {
                    resultSet.Add(MapToString((DebuggerEvent) ideEvent));
                }
                if (ideEvent is IDEStateEvent)
                {
                    resultSet.Add(MapToString((IDEStateEvent) ideEvent));
                }
                if (ideEvent is SolutionEvent)
                {
                    resultSet.Add(MapToString((SolutionEvent) ideEvent));
                }
                if (ideEvent is CompletionEvent)
                {
                    resultSet.Add(MapToString((CompletionEvent) ideEvent));
                }
            }

            if (Statistic.ContainsKey(resultSet))
            {
                Statistic[resultSet]++;
            }
            else
            {
                Statistic.Add(resultSet, 1);
            }
        }

        private static string MapToString(CommandEvent commandEvent)
        {
            return string.Format("Command{0}{1}", Separator, commandEvent.CommandId);
        }

        private static string MapToString(WindowEvent windowEvent)
        {
            return string.Format("Window{0}{1}", Separator, windowEvent.Action);
        }

        private static string MapToString(DocumentEvent documentEvent)
        {
            return string.Format("Document{0}{1}", Separator, documentEvent.Action);
        }

        private static string MapToString(BuildEvent buildEvent)
        {
            return string.Format("Build{0}{1}", Separator, buildEvent.Action);
        }

        private static string MapToString(EditEvent editEvent)
        {
            return string.Format("Edit{0}{1}", Separator, editEvent.NumberOfChanges + " Changes");
        }

        private static string MapToString(DebuggerEvent debuggerEvent)
        {
            return string.Format("Debugger{0}{1}", Separator, debuggerEvent.Reason);
        }

        private static string MapToString(IDEStateEvent ideStateEvent)
        {
            return string.Format("IDEState{0}{1}", Separator, ideStateEvent.IDELifecyclePhase);
        }

        private static string MapToString(SolutionEvent solutionEvent)
        {
            return string.Format("Solution{0}{1}", Separator, solutionEvent.Action);
        }

        private static string MapToString(ICompletionEvent completionEvent)
        {
            return string.Format("Completion{0}{1}", Separator, "Terminated as " + completionEvent.TerminatedState);
        }

        public void OnStreamStarts(Developer value) {}

        public void OnStreamEnds() {}
    }
}
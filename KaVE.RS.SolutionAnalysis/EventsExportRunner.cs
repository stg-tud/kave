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
using System.IO;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.IO.Archives;
using KaVE.Commons.Utils.Json;
using KaVE.RS.SolutionAnalysis.Episodes;

namespace KaVE.RS.SolutionAnalysis
{
    internal class EventsExportRunner
    {
        public const double EventStreamDelta = 0.001;
        public const double EventStreamTimeout = 0.5;

        private readonly string _dirCtxs;
        private readonly string _dirEvents;

        public EventsExportRunner(string dirCtxs, string dirEvents)
        {
            _dirCtxs = dirCtxs;
            _dirEvents = dirEvents;
        }

        public void Run()
        {
            var events = Lists.NewList<Event>();

            events.Add(
                new Event
                {
                    Kind = EventKind.MethodDeclaration,
                    Method = Names.Method("[You, Can] [Safely, Ignore].ThisDummyValue()")
                });

            ExtractEvents(events);
            WriteEvents(events);
        }

        private void ExtractEvents(IKaVEList<Event> events)
        {
            var logs = FindSSTLogs(_dirCtxs);
            var generator = new EventStreamGenerator();

            foreach (var log in logs)
            {
                Console.WriteLine("##################################################");
                Console.WriteLine("reading {0}...", Path.GetFileName(log));
                using (var ra = new ReadingArchive(log))
                {
                    var ctxs = ra.GetAll<Context>();
                    Console.WriteLine("\tFound {0} contexts", ctxs.Count);
                    Console.Write("\tExtracting events... ");
                    foreach (var ctx in ctxs)
                    {
                        ctx.SST.Accept(generator, events);
                        Console.Write('.');
                    }
                    Console.WriteLine(" done");
                }
            }
        }

        private IEnumerable<string> FindSSTLogs(string root)
        {
            Console.WriteLine("find contexts in '{0}'", root);
            var logs = Directory.GetFiles(root, "*-contexts.zip", SearchOption.AllDirectories);
            Console.WriteLine("available logs:");
            foreach (var log in logs)
            {
                Console.WriteLine("\t* {0}", log);
            }
            return logs;
        }

        private void WriteEvents(IKaVEList<Event> events)
        {
            var streamFileName = Path.Combine(_dirEvents, "eventstream.txt");
            var streamFile = new StreamWriter(streamFileName, false);
            var setFileName = Path.Combine(_dirEvents, "eventMapping.txt");
            var setFile = new StreamWriter(setFileName, false);

            Console.WriteLine("stream contains {0} events", events.Count);
            var uniqueEvents = new HashSet<Event>();
            var eventList = new List<Event>();

            var isFirstMethod = true;
            var time = 0.000;

            foreach (var e in events)
            {
                if (!uniqueEvents.Contains(e))
                {
                    uniqueEvents.Add(e);
                    eventList.Add(e);
                }

                var idx = eventList.IndexOf(e);

                if (e.Kind == EventKind.MethodDeclaration && !isFirstMethod)
                {
                    time += EventStreamTimeout;
                }
                isFirstMethod = false;

                streamFile.Write("{0},{1:0.000}\n", idx, time);

                time += EventStreamDelta;
            }

            streamFile.Close();

            setFile.WriteLine('[');
            var isFirst = true;
            foreach (var e in eventList)
            {
                if (!isFirst)
                {
                    setFile.WriteLine(',');
                }
                setFile.Write(e.ToCompactJson());
                isFirst = false;
            }
            setFile.WriteLine();
            setFile.WriteLine(']');

            // -> write eventList

            setFile.Close();

            Console.WriteLine("finished");
        }
    }
}
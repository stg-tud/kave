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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Utils.IO;

namespace KaVE.RS.SolutionAnalysis
{
    internal class EditLocationRunner
    {
        private readonly RelativeEditLocationAnalysis _locationAnalysis = new RelativeEditLocationAnalysis();
        private readonly string _root;

        private readonly Histogram _histogram2 = new Histogram(2);
        private readonly Histogram _histogram3 = new Histogram(3);
        private readonly Histogram _histogram4 = new Histogram(4);
        private readonly Histogram _histogram5 = new Histogram(5);
        private readonly Histogram _histogram6 = new Histogram(6);
        private readonly MergingHistogram _histogram7P = new MergingHistogram(7);

        public EditLocationRunner(string root)
        {
            _root = root;
        }

        private static void Log(string message, params object[] args)
        {
            Console.Write(@"{0} | ", DateTime.Now);
            Console.WriteLine(message, args);
        }

        public void Run()
        {
            var zips = FindFeedbackZips();
            Log(@"found {0} zips:", zips.Count);
            foreach (var zip in zips)
            {
                Log(@"- {0}", zip);
            }

            int numTotal = zips.Count;
            int numEvents = 0;
            int numCompletionEvents = 0;
            int numCurrent = 1;

            foreach (var zip in zips)
            {
                Log(
                    @"####### reading {0}/{1} #######################################",
                    numCurrent++,
                    numTotal);
                Log(@"zip: {0}", zip);

                using (var ra = new ReadingArchive(zip))
                {
                    Log(@"{0} events: ", ra.Count);
                    while (ra.HasNext())
                    {
                        var @event = ra.GetNext<IDEEvent>();
                        numEvents++;

                        var complEvent = @event as CompletionEvent;
                        if (complEvent == null)
                        {
                            Console.Write('.');
                            continue;
                        }

                        var fileName = complEvent.ActiveDocument.FileName;
                        if (fileName != null && !fileName.EndsWith(".cs"))
                        {
                            Console.Write(':');
                            continue;
                        }

                        numCompletionEvents++;

                        var loc = _locationAnalysis.Analyze(complEvent.Context2.SST);
                        if (!loc.HasEditLocation || loc.Size < 2)
                        {
                            Console.Write('o');
                            continue;
                        }

                        Console.Write('x');

                        switch (loc.Size)
                        {
                            case 2:
                                _histogram2.Add(loc.Location);
                                break;
                            case 3:
                                _histogram3.Add(loc.Location);
                                break;
                            case 4:
                                _histogram4.Add(loc.Location);
                                break;
                            case 5:
                                _histogram4.Add(loc.Location);
                                break;
                            case 6:
                                _histogram4.Add(loc.Location);
                                break;
                            default:
                                _histogram7P.AddRatio(loc.Location, loc.Size);
                                break;
                        }
                        Console.WriteLine(@" done!");
                    }
                }
            }

            Log("finished analyzing {0} events ({1} completion events)", numEvents, numCompletionEvents);

            Print("histogram 2:", _histogram2);
            Print("histogram 3:", _histogram3);
            Print("histogram 4:", _histogram4);
            Print("histogram 5:", _histogram5);
            Print("histogram 6:", _histogram6);
            Print("histogram 7+:", _histogram7P);
        }

        private void Print(string title, Histogram h)
        {
            Console.WriteLine(@"### {0} ({1} values)", title, h.NumValues);
            var values = h.ValuesRelative;
            var valuesAbs = h.Values;
            foreach (var k in values.Keys)
            {
                Console.WriteLine(@"{0}: {1,5:0.0}% ({2}x)", k, (values[k]*100), valuesAbs[k]);
            }
            Console.WriteLine();
        }


        private IList<string> FindFeedbackZips()
        {
            return Directory.GetFiles(_root, "*.zip", SearchOption.AllDirectories);
        }

        private IList<string> FindJsonFiles(string tmp)
        {
            return Directory.GetFiles(tmp, "*.json", SearchOption.TopDirectoryOnly);
        }

        public static string GetTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
    }
}
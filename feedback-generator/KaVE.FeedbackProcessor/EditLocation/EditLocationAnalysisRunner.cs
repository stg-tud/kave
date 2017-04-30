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
using KaVE.Commons.Utils.Histograms;
using KaVE.Commons.Utils.IO.Archives;

namespace KaVE.FeedbackProcessor.EditLocation
{
    internal class EditLocationAnalysisRunner
    {
        private readonly RelativeEditLocationAnalysis _locationAnalysis = new RelativeEditLocationAnalysis();
        private readonly string _root;
        private DateTime _startedAt;

        private readonly FlatHistogram _flatHist = new FlatHistogram(25);
        private readonly FlatHistogram _flatHistExec = new FlatHistogram(25);
        private readonly Histogram _histogram2 = new Histogram(2);
        private readonly Histogram _histogram3 = new Histogram(3);
        private readonly Histogram _histogram4 = new Histogram(4);
        private readonly Histogram _histogram5 = new Histogram(5);
        private readonly Histogram _histogram6 = new Histogram(6);
        private readonly Histogram _histogram7 = new Histogram(7);
        private readonly Histogram _histogram8 = new Histogram(8);
        private readonly Histogram _histogram9 = new Histogram(9);
        private readonly MergingHistogram _histogram10P = new MergingHistogram(10);

        public EditLocationAnalysisRunner(string root)
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
            _startedAt = DateTime.Now;

            var zips = FindFeedbackZips();
            Log(@"found {0} zips:", zips.Count);
            foreach (var zip in zips)
            {
                Log(@"- {0}", zip);
            }

            var numTotal = zips.Count;
            var numEvents = 0;
            var numCompletionEvents = 0;
            var numCompletionEventsApplied = 0;
            var numHistoryTuples = 0;
            var numCurrent = 1;

            Log("");
            Log("symbols:");
            Log("    . -- no completion event");
            Log("    : -- no C# file");
            Log("    o -- no edit location or empty method");
            Log("    x -- sst with edit location in non-empty method");
            Log("");

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

                        numHistoryTuples++;

                        Console.Write('x');

                        _flatHist.Add(loc.Location, loc.Size);
                        if (complEvent.TerminatedState == TerminationState.Applied)
                        {
                            numCompletionEventsApplied++;
                            _flatHistExec.Add(loc.Location, loc.Size);
                        }

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
                                _histogram5.Add(loc.Location);
                                break;
                            case 6:
                                _histogram6.Add(loc.Location);
                                break;
                            case 7:
                                _histogram7.Add(loc.Location);
                                break;
                            case 8:
                                _histogram8.Add(loc.Location);
                                break;
                            case 9:
                                _histogram9.Add(loc.Location);
                                break;
                            default:
                                _histogram10P.AddRatio(loc.Location, loc.Size);
                                break;
                        }
                    }
                }
                Console.WriteLine();
                Log("");
            }

            Log("");
            Log("started at {0}", _startedAt);
            Log(
                "finished analyzing {0} events ({1} completion events, {2} with location, {3} applied)",
                numEvents,
                numCompletionEvents,
                numHistoryTuples,
                numCompletionEventsApplied);
            Log("");

            Print("histogram 2:", _histogram2);
            Print("histogram 3:", _histogram3);
            Print("histogram 4:", _histogram4);
            Print("histogram 5:", _histogram5);
            Print("histogram 6:", _histogram6);
            Print("histogram 7:", _histogram7);
            Print("histogram 8:", _histogram8);
            Print("histogram 9:", _histogram9);
            Print("histogram 10+:", _histogram10P);

            Console.WriteLine(@"flat histogram:");
            Console.WriteLine(_flatHist);

            Console.WriteLine(@"flat histogram (applied):");
            Console.WriteLine(_flatHistExec);

            HistToCsv(_flatHist);
            HistToCsv(_flatHistExec);
        }

        private static void HistToCsv(FlatHistogram hist)
        {
            Console.WriteLine();
            var bins = hist.GetBins();
            for (var i = 0; i < bins.Length; i++)
            {
                Console.WriteLine(@"{0} {1}", i, bins[i]);
            }
        }

        private void Print(string title, Histogram h)
        {
            Console.WriteLine(@"### {0} ({1} values)", title, h.NumValues);
            var values = h.ValuesRelative;
            var valuesAbs = h.Values;
            foreach (var k in values.Keys)
            {
                Console.WriteLine(@"{0}: {1,5:0.0}% ({2}x)", k, (values[k] * 100), valuesAbs[k]);
            }
            Console.WriteLine();
        }


        private IList<string> FindFeedbackZips()
        {
            return Directory.GetFiles(_root, "*.zip", SearchOption.AllDirectories);
        }

        public static string GetTemporaryDirectory()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
    }
}
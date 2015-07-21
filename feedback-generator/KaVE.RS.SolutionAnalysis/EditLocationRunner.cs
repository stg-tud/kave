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
using Ionic.Zip;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Utils.Json;

namespace KaVE.RS.SolutionAnalysis
{
    internal class EditLocationRunner
    {
        private readonly RelativeEditLocationAnalysis _locationAnalysis = new RelativeEditLocationAnalysis();
        private readonly string _root;

        private readonly Histogram _histogram2 = new Histogram(2);
        private readonly Histogram _histogram3 = new Histogram(3);
        private readonly Histogram _histogram4 = new Histogram(4);
        private readonly MergingHistogram _histogram5P = new MergingHistogram(5);

        public EditLocationRunner(string root)
        {
            _root = root;
        }

        public void Run()
        {
            var zips = FindFeedbackZips();
            int numEvents = 0;
            int numCompletionEvents = 0;
            int numTotal = zips.Count;
            int numCurrent = 1;
            Console.WriteLine(@"found {0} zips:", numTotal);
            foreach (var zip in zips)
            {
                Console.WriteLine(@"- {0}", zip);
            }


            foreach (var zip in zips)
            {
                var tmp = GetTemporaryDirectory();

                try
                {
                    Console.WriteLine(
                        @"####### reading {0}/{1} #######################################",
                        numCurrent++,
                        numTotal);
                    Console.WriteLine(@"[{1}] zip: {0}", zip, DateTime.Now);


                    using (var zipFile = ZipFile.Read(zip))
                    {
                        zipFile.ExtractAll(tmp);

                        var jsonFiles = FindJsonFiles(tmp);
                        Console.Write(@"{0} events: ", jsonFiles.Count);

                        foreach (var f in jsonFiles)
                        {
                            Console.Write('.');
                            var json = File.ReadAllText(f);
                            var @event = json.ParseJsonTo<IDEEvent>();
                            numEvents++;

                            var complEvent = @event as CompletionEvent;
                            if (complEvent == null)
                            {
                                Console.Write('.');
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
                                default:
                                    _histogram5P.AddRatio(loc.Location, loc.Size);
                                    break;
                            }
                        }
                        Console.WriteLine(@" done!");
                    }
                }
                finally
                {
                    Directory.Delete(tmp, true);
                }
            }


            Console.WriteLine("finished analyzing {0} events ({1} completion events)", numEvents, numCompletionEvents);

            Print("histogram 2:", _histogram2);
            Print("histogram 3:", _histogram3);
            Print("histogram 4:", _histogram4);
            Print("histogram 5+:", _histogram5P);
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
            return Directory.GetFiles(_root, "*.zip", SearchOption.TopDirectoryOnly);
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
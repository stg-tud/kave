/*
 * Copyright 2017 Sebastian Proksch
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
using System.Diagnostics.CodeAnalysis;
using KaVE.Commons.Utils.Histograms;
using KaVE.FeedbackProcessor.StatisticsUltimate;

namespace KaVE.FeedbackProcessor.EditLocation
{
    public interface IEditLocationAnalysisLogger : IStatisticsLogger
    {
        void IntermediateResults(int taskId, IEditLocationResults stats);
        void FinalResults(IEditLocationResults results);
    }

    [SuppressMessage("ReSharper", "LocalizableElement")]
    internal class EditLocationAnalysisLogger : StatisticsLoggerBase, IEditLocationAnalysisLogger
    {
        public void IntermediateResults(int taskId, IEditLocationResults res)
        {
            lock (Lock)
            {
                Log("({0}) Intermediate Result:\n", taskId);
                LogRes(res);
            }
        }

        private readonly FlatHistogram _flatHistAll = new FlatHistogram(25);
        private readonly FlatHistogram _flatHistApplied = new FlatHistogram(25);
        private readonly FlatHistogram _flatHistOther = new FlatHistogram(25);

        private readonly Histogram _histogram2 = new Histogram(2);
        private readonly Histogram _histogram3 = new Histogram(3);
        private readonly Histogram _histogram4 = new Histogram(4);
        private readonly Histogram _histogram5 = new Histogram(5);
        private readonly Histogram _histogram6 = new Histogram(6);
        private readonly Histogram _histogram7 = new Histogram(7);
        private readonly Histogram _histogram8 = new Histogram(8);
        private readonly Histogram _histogram9 = new Histogram(9);
        private readonly MergingHistogram _histogram10P = new MergingHistogram(10);

        public void FinalResults(IEditLocationResults res)
        {
            lock (Lock)
            {
                Log("Final Result:\n");
                LogRes(res);

                foreach (var loc in res.AppliedEditLocations)
                {
                    _flatHistAll.Add(loc.Location, loc.Size);
                    _flatHistApplied.Add(loc.Location, loc.Size);
                    SortIn(loc);
                }
                foreach (var loc in res.OtherEditLocations)
                {
                    _flatHistAll.Add(loc.Location, loc.Size);
                    _flatHistOther.Add(loc.Location, loc.Size);
                    SortIn(loc);
                }

                Print("histogram 2:", _histogram2);
                Print("histogram 3:", _histogram3);
                Print("histogram 4:", _histogram4);
                Print("histogram 5:", _histogram5);
                Print("histogram 6:", _histogram6);
                Print("histogram 7:", _histogram7);
                Print("histogram 8:", _histogram8);
                Print("histogram 9:", _histogram9);
                Print("histogram 10+:", _histogram10P);

                Console.WriteLine(@"flat histogram (all):");
                Console.WriteLine(_flatHistAll);

                Console.WriteLine(@"flat histogram (applied):");
                Console.WriteLine(_flatHistApplied);

                Console.WriteLine(@"flat histogram (other):");
                Console.WriteLine(_flatHistOther);

                HistToCsv("all", _flatHistAll);
                HistToCsv("applied", _flatHistApplied);
                HistToCsv("other", _flatHistOther);
            }
        }

        private static void LogRes(IEditLocationResults res)
        {
            var tmp = new EditLocationResults {Zip = res.Zip};
            tmp.Add(res);
            tmp.AppliedEditLocations.Clear();
            tmp.OtherEditLocations.Clear();

            Console.WriteLine("NumAppliedEditLocations = {0}", res.AppliedEditLocations.Count);
            Console.WriteLine("NumOtherEditLocations = {0}", res.OtherEditLocations.Count);
            Console.Write(tmp.ToString());
        }

        private void SortIn(RelativeEditLocation loc)
        {
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

        private static void Print(string title, Histogram h)
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

        private static void HistToCsv(string title, FlatHistogram hist)
        {
            Console.WriteLine(@"### {0}", title);
            Console.WriteLine();
            var bins = hist.GetBins();
            for (var i = 0; i < bins.Length; i++)
            {
                Console.WriteLine("{0}\t{1}", i, bins[i]);
            }
            Console.WriteLine();
        }
    }
}
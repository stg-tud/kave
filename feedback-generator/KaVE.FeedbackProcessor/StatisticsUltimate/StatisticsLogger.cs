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

namespace KaVE.FeedbackProcessor.StatisticsUltimate
{
    public class StatisticsLogger
    {
        private readonly object _lock = new object();

        public void SearchingZips(string dirIn)
        {
            lock (_lock)
            {
                Log("Searching for .zip files in '{0}' ...", dirIn);
            }
        }

        private int _total;

        public void FoundZips(int count)
        {
            lock (_lock)
            {
                _total = count;
                Append(" found {0} files", count);
            }
        }

        public void StartingStatCreation(int taskId)
        {
            lock (_lock)
            {
                Log("({0}) StartingStatCreation", taskId);
            }
        }

        private int _current;

        public void CreatingStats(int taskId, string zip)
        {
            lock (_lock)
            {
                _current++;
                var perc = 100*_current/(double) _total;
                Log("({0}) CreatingStats for {1} ({2}/{3} started -- {4:0.0}%)", taskId, zip, _current, _total, perc);
            }
        }

        public void FinishedStatCreation(int taskId)
        {
            lock (_lock)
            {
                Log("({0}) FinishedStatCreation", taskId);
            }
        }

        public void Result(IDictionary<string, UserStatistics> results)
        {
            lock (_lock)
            {
                Log("done!");

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine(
                    "zip\tDayFirst\tDayLast\tNumDays\tNumMonths\tNumEvents\tEducation\tPosition\tNumCodeCompletion\tNumTestRuns");
                foreach (var zip in results.Keys)
                {
                    var stats = results[zip];
                    Console.WriteLine(
                        "{0}\t{1:MM/dd/yy}\t{2:MM/dd/yy}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}",
                        zip,
                        stats.DayFirst,
                        stats.DayLast,
                        stats.NumDays,
                        stats.NumMonth,
                        stats.NumEvents,
                        stats.Education,
                        stats.Position,
                        stats.NumCodeCompletion,
                        stats.NumTestRuns);
                }
            }
        }

        private static void Append(string msg, params object[] args)
        {
            Console.Write(msg, args);
        }

        private bool _isFirstLine = true;

        private void Log(string msg, params object[] args)
        {
            if (!_isFirstLine)
            {
                Console.WriteLine();
            }
            _isFirstLine = false;
            Console.Write(DateTime.Now);
            Console.Write('\t');
            Append(msg, args);
        }
    }
}
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
using System.Diagnostics.CodeAnalysis;

namespace KaVE.FeedbackProcessor.StatisticsUltimate
{
    public class InteractionStatisticsLogger : StatisticsLoggerBase
    {
        public void ReportTimeout()
        {
            lock (Lock)
            {
                Log(
                    "Using an interaction timeout of {0}s to merge active times ...",
                    InteractionStatisticsExtractor.TimeOutInS);
            }
        }

        [SuppressMessage("ReSharper", "LocalizableElement")]
        public void Result(IDictionary<string, InteractionStatistics> results)
        {
            lock (Lock)
            {
                Log("done!");

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine(
                    "make sure to set the (Excel) cell format of ActiveTime to 'd.hh:mm:ss' or '[h]:mm:ss' to improve readbility");
                Console.WriteLine();
                Console.WriteLine(
                    "zip\tDayFirst\tDayLast\tNumDays\tNumMonths\tNumEvents\tEducation\tPosition\tNumCodeCompletion\tNumTestRuns\tActiveTime");
                foreach (var zip in results.Keys)
                {
                    var stats = results[zip];
                    Console.WriteLine(
                        "{0}\t{1:MM/dd/yy}\t{2:MM/dd/yy}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10:00}{11:\\:mm\\:ss}",
                        zip,
                        stats.DayFirst,
                        stats.DayLast,
                        stats.NumDays,
                        stats.NumMonth,
                        stats.NumEvents,
                        stats.Education,
                        stats.Position,
                        stats.NumCodeCompletion,
                        stats.NumTestRuns,
                        stats.ActiveTime.TotalHours,
                        stats.ActiveTime);
                }
            }
        }
    }
}
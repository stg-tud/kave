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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using JetBrains.Application;
using KaVE.Commons.Model.Events;
using KaVE.VS.FeedbackGenerator.Utils.Logging;
using KaVE.VS.Statistics.Calculators.BaseClasses;
using KaVE.VS.Statistics.Utils;

namespace KaVE.VS.Statistics.LogCollector
{
    /// <summary>
    ///     Collects all Events from the LogManager
    /// </summary>
    [ShellComponent]
    public class LogReplay
    {
        public static bool LogReplayRunning;
        private readonly IEnumerable<IStatisticCalculator> _calculatorList;

        private readonly IEnumerable<ILog> _logs;
        private readonly IStatisticListing _statisticListing;

        public LogReplay(ILogManager logManager, IStatisticListing statisticListing)
        {
            _statisticListing = statisticListing;
            _calculatorList = Registry.GetComponents<IStatisticCalculator>();
            _logs = logManager.Logs;
        }

        public void SendEventsToCalculators(IEnumerable<IDEEvent> events)
        {
            foreach (var ideEvent in events)
            {
                foreach (var statisticCalculator in _calculatorList)
                {
                    statisticCalculator.Event(ideEvent);
                }
            }
        }

        /// <summary>
        ///     Collects all Events from logs and sends them to all Calculators;
        ///     Postpones updates to the Listings Observers until after collecting
        /// </summary>
        /// <param name="worker">The BackgroundWorker used for showing the current progress of collecting</param>
        public void CollectEventsFromLog(BackgroundWorker worker)
        {
            _statisticListing.BlockUpdateToObservers = true;
            LogReplayRunning = true;

            var index = 0;
            var length = _logs.Count();

            foreach (var log in _logs)
            {
                CalculateAndReportProgress(worker, index, length);
                SendEventsToCalculators(log.ReadAll());
                index++;
            }

            _statisticListing.BlockUpdateToObservers = false;
            _statisticListing.SendUpdateToObserversWithAllStatistics();
            LogReplayRunning = false;
        }

        private static void CalculateAndReportProgress(BackgroundWorker worker, int index, int length)
        {
            var progress = 100*index/length;
            worker.ReportProgress(progress);
        }
    }
}
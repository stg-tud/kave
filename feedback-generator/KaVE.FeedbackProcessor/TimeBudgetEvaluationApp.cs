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
using System.Linq;
using KaVE.Commons.Utils.Csv;
using KaVE.Commons.Utils.DateTime;
using KaVE.Commons.Utils.Exceptions;
using KaVE.FeedbackProcessor.Activities;
using KaVE.FeedbackProcessor.Activities.Intervals;
using KaVE.FeedbackProcessor.Activities.SlidingWindow;
using KaVE.FeedbackProcessor.Cleanup.Processors;
using KaVE.FeedbackProcessor.Database;
using KaVE.FeedbackProcessor.Import;
using KaVE.FeedbackProcessor.Properties;
using KaVE.FeedbackProcessor.Statistics;
using KaVE.FeedbackProcessor.Utils;
using KaVE.FeedbackProcessor.VsWindows;

namespace KaVE.FeedbackProcessor
{
    internal class TimeBudgetEvaluationApp
    {
        private readonly ILogger _logger;

        public TimeBudgetEvaluationApp(ILogger logger)
        {
            _logger = logger;
        }

        public void Run()
        {
            RunWithErrorLogging(
                () =>
                {
                    const string importDatabase = "_import";
                    const string filteredDatabase = "_filtered";
                    const string cleanDatabase = "_clean";
                    const string activityDatabase = "_activities";
                    const string concurrentEventDatabase = "_concurrent";
                    const string commandFollowupsDatabase = "_commandFollowups";

                    //ImportFeedback(OpenDatabase(importDatabase));

                    // cleanup as used for ICSME paper version
                    //CleanupFeedback(OpenDatabase(importDatabase), OpenDatabase(cleanDatabase));

                    // statistics computed to explore dataset, not used in papers
                    //SelectConcurrentEvents(OpenDatabase(filteredDatabase), OpenDatabase(concurrentEventDatabase));
                    //ConcurrentEventsStatistic(OpenDatabase(concurrentEventDatabase), "concurrenteventstatistic.csv");
                    //SelectCommandFollowupPairs(OpenDatabase(filteredDatabase), OpenDatabase(commandFollowupsDatabase));
                    //ConcurrentEventsStatistic(OpenDatabase(commandFollowupsDatabase), "commandfollowupsstatistic.csv");

                    //LogAnonymizationStatistics(OpenDatabase(importDatabase));
                    //LogDeveloperStatistics(OpenDatabase(importDatabase));
                    //LogEventsPerDeveloperDayStatistic(OpenDatabase(importDatabase));
                    //LogIDEActivationEvents(OpenDatabase(importDatabase));
                    //LogOccuringNames(OpenDatabase(cleanDatabase));
                    //LogCompletionStatistics(OpenDatabase(importDatabase));
                    //LogDevelopersPerDay(OpenDatabase(importDatabase));
                    //LogAverageBreakAfterEventsStatistic(OpenDatabase(cleanDatabase), "averageBreakAfterEvents.csv");

                    RunActivityAnalysis(OpenDatabase(cleanDatabase), OpenDatabase(activityDatabase));
                    //RunWindowUsageAnalysis(OpenDatabase(activityDatabase));
                });
        }

        private void RunActivityAnalysis(IFeedbackDatabase cleanDatabase, IFeedbackDatabase activityDatabase)
        {
            //MapToActivities(cleanDatabase, activityDatabase);
            //LogActivityWindowStatistics(activityDatabase);
            LogActivityIntervalStatistics(activityDatabase);
        }

        private void RunWindowUsageAnalysis(IFeedbackDatabase activityDatabase)
        {
            var windowIntervalProcessor = new WindowIntervalProcessor(TimeSpan.FromHours(4));

            var processor = new FeedbackProcessor(activityDatabase, _logger);
            processor.Register(windowIntervalProcessor);
            processor.ProcessFeedback();

            Output(
                    "window-budget-per-developer-(intervals).csv",
                    windowIntervalProcessor.IntervalsToVsWindowUsageStatisticCsv());
        }

        # region Cleanup Feedback

        private void CleanupFeedback(IFeedbackDatabase importDatabase, IFeedbackDatabase cleanDatabase)
        {
            var tmpDatabase = OpenDatabase("_tmp");

            //DoSimpleCleanup(importDatabase, tmpDatabase);
            // NOTE: Statistics need to be cleaned before advanced cleanup is performed!
            //ComputeAdvancedCleanupStatistics(tmpDatabase);
            DoAdvancedCleanup(tmpDatabase, cleanDatabase);
        }

        private void DoSimpleCleanup(IFeedbackDatabase importDatabase, IFeedbackDatabase tmpDatabase)
        {
            var cleaner = new FeedbackMapper(importDatabase, tmpDatabase, _logger);
            cleaner.RegisterMapper(new ErrorFilterProcessor());
            cleaner.RegisterMapper(new UnnamedCommandFilterProcessor());
            cleaner.RegisterMapper(new DuplicateCommandFilterProcessor());
            cleaner.RegisterMapper(new RedundantCommandFilter());
            //cleaner.RegisterMapper(new AddFileProcessor());
            //cleaner.RegisterMapper(new EditFilterProcessor());
            cleaner.MapFeedback();
        }

        private void ComputeAdvancedCleanupStatistics(IFeedbackDatabase tmpDatabase)
        {
            var isolatedEventBlocksCalculator = new IsolatedEventBlocksCalculator(
                TimeSpan.FromMinutes(30),
                TimeSpan.FromSeconds(1));
            var equivalentCommandPairCalculator = new EquivalentCommandPairCalculator(0);

            var walker = new FeedbackProcessor(tmpDatabase, _logger);
            walker.Register(isolatedEventBlocksCalculator);
            walker.Register(equivalentCommandPairCalculator);
            walker.ProcessFeedback();

            Output("isolatedEventBlocks.txt", isolatedEventBlocksCalculator.LoggedIsolatedBlocksToTxt());
            Output("equivalentCommandPairs.csv", equivalentCommandPairCalculator.StatisticAsCsv());
        }

        private void DoAdvancedCleanup(IFeedbackDatabase tmpDatabase, IFeedbackDatabase cleanDatabase)
        {
            var mapper = new FeedbackMapper(tmpDatabase, cleanDatabase, _logger);
            mapper.RegisterMapper(new IsolatedEventBlockFilter(TimeSpan.FromMinutes(30), TimeSpan.FromSeconds(1)));
            mapper.RegisterMapper(new MapEquivalentCommandsProcessor(new ResourceProvider()));
            mapper.MapFeedback();
        }

        # endregion

        private void LogCompletionStatistics(IFeedbackDatabase database)
        {
            var invocations = new CompletionInvocationStatistic();
            var durations = new CompletionDurationStatistic();
            var manualDurations = new ManualCompletionsStatistic();

            var processor = new FeedbackProcessor(database, _logger);
            processor.Register(invocations);
            processor.Register(durations);
            processor.Register(manualDurations);
            processor.ProcessFeedback();

            _logger.Info("Found {0} completion invocations.", invocations.CompletionInvocations);
            Output("completion-durations.csv", durations.StatisticAsCsv());
            _logger.Info(
                "Found {0} developer days with completion usage.",
                durations.NumberOfDeveloperDaysWithCompletionUsage);

            _logger.Info(
                "Found {0} milliseconds of manual completion usage.",
                manualDurations.DurationInManualCompletion.TotalMilliseconds);
            _logger.Info("Found {0} manual completion usages.", manualDurations.NumberOfManualCompletions);
            _logger.Info("Found {0} applied manual completion usages.", manualDurations.NumberOfAppliedCompletions);
            _logger.Info("Found {0} cancelled manual completion usages.", manualDurations.NumberOfCancelledCompletions);
            _logger.Info(
                "Found manual completion usages on {0} developer days.",
                manualDurations.NumberOfDeveloperDaysWithManualCompletionUsage);
        }

        private void LogDevelopersPerDay(IFeedbackDatabase database)
        {
            var calculator = new DevelopersPerDayCalculator();

            var feedbackProcessor = new FeedbackProcessor(database, _logger);
            feedbackProcessor.Register(calculator);
            feedbackProcessor.ProcessFeedback();

            Output("developers-per-day.csv", calculator.GetStatisticAsCsv());
        }

        #region Helper

        private IFeedbackDatabase OpenDatabase(string databaseSuffix)
        {
            return MongoDbFeedbackDatabase.Open(Configuration.DatabaseUrl, Configuration.DatasetName, databaseSuffix);
        }

        private void Output(string outputFilename, string text)
        {
            File.WriteAllText(OutputFilename(outputFilename), text);
        }

        private void Output(string outputFilename, IEnumerable<string> lines)
        {
            File.WriteAllLines(OutputFilename(outputFilename), lines);
        }

        private string OutputFilename(string developerdaystatsCsv)
        {
            return Path.Combine(Configuration.StatisticsOutputPath, developerdaystatsCsv);
        }
        
        #endregion

        private void ImportFeedback(IFeedbackDatabase importDatabase)
        {
            var feedbackImporter = new FeedbackImporter(importDatabase, _logger);
            feedbackImporter.Import();
        }

        public void LogAnonymizationStatistics(IFeedbackDatabase database)
        {
            var calculator = new AnonymizationStatisticsLogger(database, _logger);
            calculator.LogNumberOfEventsWithoutSessionId();
            calculator.LogNumberOfEventsWithoutTriggerTime();
            calculator.LogNumberOfEventsWithoutDuration();
            calculator.LogNumberOfSessionsWithAnonymizedNames();
        }

        public void LogDeveloperStatistics(IFeedbackDatabase database)
        {
            var stats = new DeveloperStatisticsCalculator(database);
            _logger.Info("We have at most {0} participant(s).", stats.GetUpperBoundToNumberOfParticipants());
            _logger.Info("We have at least {0} participant(s).", stats.GetLowerBoundToNumberOfParticipants());
            _logger.Info("We have {0} session(s) in total.", stats.GetNumberOfSessions());
            var conflicts = stats.GetNumberOfSessionsAssignedToMultipleDevelopers();
            if (conflicts > 0)
            {
                _logger.Error("We have {0} session(s) assigned to more than one developer!", conflicts);
            }
        }

        private void LogEventsPerDeveloperDayStatistic(IFeedbackDatabase database)
        {
            var calculator5 = new EventsPerDeveloperDayStatisticCalculator(TimeSpan.FromMinutes(5));
            var calculator10 = new EventsPerDeveloperDayStatisticCalculator(TimeSpan.FromMinutes(10));

            var walker = new FeedbackProcessor(database, _logger);
            walker.Register(calculator5);
            walker.Register(calculator10);
            walker.ProcessFeedback();

            Output("events-per-developer.csv", calculator5.EventsPerDeveloperStatisticAsCsv());
            //Output("event-per-developer-day.csv", calculator5.EventsPerDeveloperDayStatisticAsCsv());
        }

        private void LogOccuringNames(IFeedbackDatabase database)
        {
            var windowNameCollector = new WindowNameCollector();
            var documentNameCollector = new DocumentNameCollector();
            var commandIdCollector = new CommandIdCollector();

            var walker = new FeedbackProcessor(database, _logger);
            walker.Register(windowNameCollector);
            walker.Register(documentNameCollector);
            walker.Register(commandIdCollector);
            walker.ProcessFeedback();

            Output("windownames.csv", windowNameCollector.AllWindowNames.EntryDictionary.ToCsv());
            Output("documentnames.csv", documentNameCollector.AllDocumentNames.EntryDictionary.ToCsv());
            Output("commandids.csv", commandIdCollector.AllCommandIds.EntryDictionary.ToCsv());
        }

        private void LogIDEActivationEvents(IFeedbackDatabase database)
        {
            var calculator = new ParallelIDEInstancesStatisticCalculator();

            var walker = new FeedbackProcessor(database, _logger);
            walker.Register(calculator);
            walker.ProcessFeedback();

            Output("window-de-activation.log", calculator.Statistic);
        }

        #region Activities

        private void MapToActivities(IFeedbackDatabase sourceDatabase, IFeedbackDatabase activityDatabase)
        {
            var activityMapper = new FeedbackMapper(sourceDatabase, activityDatabase, _logger);
            activityMapper.RegisterMapper(new AlwaysDropMapper()); // only generated events reach activity database
            activityMapper.RegisterMapper(new AnyToActivityMapper()); // map any event to a keep-alive
            activityMapper.RegisterMapper(new BuildEventToActivityMapper());
            activityMapper.RegisterMapper(new CommandEventToActivityMapper());
            activityMapper.RegisterMapper(new CompletionEventToActivityMapper());
            activityMapper.RegisterMapper(new DebuggerEventToActivityMapper());
            activityMapper.RegisterMapper(new DocumentEventToActivityMapper());
            activityMapper.RegisterMapper(new EditEventToActivityMapper());
            activityMapper.RegisterMapper(new FindEventToActivityMapper());
            activityMapper.RegisterMapper(new IDEStateEventToActivityMapper());
            activityMapper.RegisterMapper(new InstallEventToActivityMapper());
            activityMapper.RegisterMapper(new SolutionEventToActivityMapper());
            activityMapper.RegisterMapper(new UpdateEventToActivityMapper());
            activityMapper.RegisterMapper(new WindowEventToActivityMapper(_logger));
            activityMapper.MapFeedback();
        }

        private void LogActivityWindowStatistics(IFeedbackDatabase activityDatabase)
        {
            var activityWindowProcessor1 = new ActivityWindowProcessor(
                new FrequencyActivityMergeStrategy(),
                TimeSpan.FromMilliseconds(1000));

            var processor = new FeedbackProcessor(activityDatabase, _logger);
            processor.Register(activityWindowProcessor1);
            processor.ProcessFeedback();

            Output(
                "developer-activities-1-B5-S15.csv",
                activityWindowProcessor1.ActivityStreamsToCsv(TimeSpan.FromSeconds(15), TimeSpan.FromMinutes(5)));
            Output(
                "devday-activities-1-B5-S15.csv",
                activityWindowProcessor1.DeveloperDayStatisticToCsv(TimeSpan.FromSeconds(15), TimeSpan.FromMinutes(5)));
            
            Output(
                "inactivity-separation-1000-0.csv",
                activityWindowProcessor1.InactivityStatisticToCsv(
                    TimeSpan.Zero,
                    Enumerable.Range(1, 20).Select(i => TimeSpan.FromSeconds(i)).ToArray()));

            Output(
                "inactivity-separation-1000-15.csv",
                activityWindowProcessor1.InactivityStatisticToCsv(
                    TimeSpan.FromSeconds(15),
                    Enumerable.Range(1, 20).Select(i => TimeSpan.FromMinutes(i)).ToArray()));
        }

        private void LogActivityIntervalStatistics(IFeedbackDatabase activityDatabase)
        {
            var activityIntervalProcessor = new ActivityIntervalProcessor(TimeSpan.FromHours(4));

            var processor = new FeedbackProcessor(activityDatabase, _logger);
            processor.Register(activityIntervalProcessor);
            processor.ProcessFeedback();
            
            Output(
                    "activity-budget-per-developer-(intervals)-15-05.csv",
                    activityIntervalProcessor.IntervalsToDeveloperBudgetCsv(
                        TimeSpan.FromSeconds(15),
                        TimeSpan.FromMinutes(5)));

            /*Output(
                "inactivity-separation-1-n.csv",
                activityIntervalProcessor.InactivityStatisticToCsv(
                    TimeSpan.FromSeconds(1),
                    Enumerable.Range(2, 20).Select(i => TimeSpan.FromSeconds(i)).ToArray()));

            Output(
                "inactivity-seperation-15-n.csv",
                activityIntervalProcessor.InactivityStatisticToCsv(
                    TimeSpan.FromSeconds(15),
                    Enumerable.Range(1, 20).Select(i => TimeSpan.FromMinutes(i)).ToArray()));

            foreach (var lostTime in activityIntervalProcessor.LostTimeStatistics)
            {
                _logger.Info(
                    "For activity '{0}' we lost {1}s on {2} events ({3}s on average).",
                    lostTime.Key,
                    lostTime.Value.Time.RoundedTotalSeconds(),
                    lostTime.Value.Frequency,
                    (lostTime.Value.Time.TotalSeconds / lostTime.Value.Frequency));
            }

            foreach (var overlappingActivity in activityIntervalProcessor.OverlappingActivities)
            {
                _logger.Info("Activity '{0}' was overlapped by '{1}'.", overlappingActivity.Item1, overlappingActivity.Item2);
            }*/
        }

        #endregion

        private void LogAverageBreakAfterEventsStatistic(IFeedbackDatabase database, string fileName)
        {
            var calculator = new AverageBreakAfterEventsCalculator(5, TimeSpan.FromSeconds(30), true);

            var walker = new FeedbackProcessor(database, _logger);
            walker.Register(calculator);
            walker.ProcessFeedback();

            Output(fileName, calculator.StatisticAsCsv());
        }

        #region Concurrent Events

        private void SelectConcurrentEvents(IFeedbackDatabase sourceDatabase, IFeedbackDatabase targetDatabase)
        {
            var filter = new FeedbackMapper(sourceDatabase, targetDatabase, _logger);
            filter.RegisterMapper(new AlwaysDropMapper());
            filter.RegisterMapper(new ToConcurrentEventMapper());
            filter.MapFeedback();
        }

        private void ConcurrentEventsStatistic(IFeedbackDatabase database, string fileName)
        {
            var calculator = new ConcurrentSetsCalculator();

            var walker = new FeedbackProcessor(database, _logger);
            walker.Register(calculator);
            walker.ProcessFeedback();

            Output(fileName, calculator.StatisticAsCsv());
        }

        private void SelectCommandFollowupPairs(IFeedbackDatabase sourceDatabase,
            IFeedbackDatabase targetDatabase)
        {
            var filter = new FeedbackMapper(sourceDatabase, targetDatabase, _logger);
            filter.RegisterMapper(new CommandFollowupProcessor());
            filter.MapFeedback();
        }

        #endregion

        private void RunWithErrorLogging(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Output("error.log", e.Message + "\n" + e.StackTrace);
            }
        }
    }
}
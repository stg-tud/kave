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
using KaVE.Commons.Utils.Csv;
using KaVE.Commons.Utils.Exceptions;
using KaVE.FeedbackProcessor.Activities;
using KaVE.FeedbackProcessor.Activities.SlidingWindow;
using KaVE.FeedbackProcessor.Cleanup.Processors;
using KaVE.FeedbackProcessor.Database;
using KaVE.FeedbackProcessor.Import;
using KaVE.FeedbackProcessor.Properties;
using KaVE.FeedbackProcessor.Statistics;
using KaVE.FeedbackProcessor.Utils;

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
            const string importDatabase = "_import";
            const string filteredDatabase = "_filtered";
            const string cleanDatabase = "_clean";
            const string activityDatabase = "_activities";
            const string concurrentEventDatabase = "_concurrent";
            const string commandFollowupsDatabase = "_commandFollowups";

            ImportFeedback(OpenDatabase(importDatabase));

            CleanupFeedback(OpenDatabase(importDatabase), OpenDatabase(cleanDatabase));

            SelectConcurrentEvents(OpenDatabase(filteredDatabase), OpenDatabase(concurrentEventDatabase));
            ConcurrentEventsStatistic(OpenDatabase(concurrentEventDatabase), "concurrenteventstatistic.csv");
            SelectCommandFollowupPairs(OpenDatabase(filteredDatabase), OpenDatabase(commandFollowupsDatabase));
            ConcurrentEventsStatistic(OpenDatabase(commandFollowupsDatabase), "commandfollowupsstatistic.csv");

            LogAnonymizationStatistics(OpenDatabase(importDatabase));
            LogDeveloperStatistics(OpenDatabase(importDatabase));
            LogEventsPerDeveloperDayStatistic(OpenDatabase(importDatabase));
            LogIDEActivationEvents(OpenDatabase(importDatabase));
            LogOccuringNames(OpenDatabase(cleanDatabase));
            LogCompletionStatistics(OpenDatabase(importDatabase));
            LogDevelopersPerDay(OpenDatabase(importDatabase));
            LogAverageBreakAfterEventsStatistic(OpenDatabase(cleanDatabase), "averageBreakAfterEvents.csv");

            MapToActivities(OpenDatabase(cleanDatabase), OpenDatabase(activityDatabase));
            LogActivityStatistics(OpenDatabase(activityDatabase));
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
            _logger.Info("Found {0} developer days with completion usage.", durations.NumberOfDeveloperDaysWithCompletionUsage);

            _logger.Info("Found {0} milliseconds of manual completion usage.", manualDurations.DurationInManualCompletion.TotalMilliseconds);
            _logger.Info("Found {0} manual completion usages.", manualDurations.NumberOfManualCompletions);
            _logger.Info("Found {0} applied manual completion usages.", manualDurations.NumberOfAppliedCompletions);
            _logger.Info("Found {0} cancelled manual completion usages.", manualDurations.NumberOfCancelledCompletions);
            _logger.Info("Found manual completion usages on {0} developer days.", manualDurations.NumberOfDeveloperDaysWithManualCompletionUsage);
        }

        private void LogDevelopersPerDay(IFeedbackDatabase database)
        {
            var calculator = new DevelopersPerDayCalculator();

            var feedbackProcessor = new FeedbackProcessor(database, _logger);
            feedbackProcessor.Register(calculator);
            feedbackProcessor.ProcessFeedback();

            Output("developers-per-day.csv", calculator.GetStatisticAsCsv());
        }

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

        private void ConcurrentEventsStatistic(IFeedbackDatabase database, string fileName)
        {
            var calculator = new ConcurrentSetsCalculator();

            var walker = new FeedbackProcessor(database, _logger);
            walker.Register(calculator);
            walker.ProcessFeedback();

            Output(fileName, calculator.StatisticAsCsv());
        }

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

        private void LogActivityStatistics(IFeedbackDatabase activityDatabase)
        {
            var activityWindowProcessor1 = new ActivityWindowProcessor(
                new FrequencyActivityMergeStrategy(),
                TimeSpan.FromMilliseconds(1000));

            var processor = new FeedbackProcessor(activityDatabase, _logger);
            processor.Register(activityWindowProcessor1);
            processor.ProcessFeedback();

            Output("developer-activities-1-B5-S15.csv", activityWindowProcessor1.ActivityStreamsToCsv(TimeSpan.FromSeconds(15), TimeSpan.FromMinutes(5)));
            Output("devday-activities-1-B5-S15.csv", activityWindowProcessor1.DeveloperDayStatisticToCsv(TimeSpan.FromSeconds(15), TimeSpan.FromMinutes(5)));

            Output(
                "inactivity-separation-1000-0.csv",
                activityWindowProcessor1.InactivityStatisticToCsv(
                    TimeSpan.Zero,
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(4),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(6),
                    TimeSpan.FromSeconds(7),
                    TimeSpan.FromSeconds(8),
                    TimeSpan.FromSeconds(9),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(11),
                    TimeSpan.FromSeconds(12),
                    TimeSpan.FromSeconds(13),
                    TimeSpan.FromSeconds(14),
                    TimeSpan.FromSeconds(15),
                    TimeSpan.FromSeconds(16),
                    TimeSpan.FromSeconds(17),
                    TimeSpan.FromSeconds(18),
                    TimeSpan.FromSeconds(19),
                    TimeSpan.FromSeconds(20)));

            Output(
                "inactivity-separation-1000-15.csv",
                activityWindowProcessor1.InactivityStatisticToCsv(
                    TimeSpan.FromSeconds(15),
                    TimeSpan.FromMinutes(1),
                    TimeSpan.FromMinutes(2),
                    TimeSpan.FromMinutes(3),
                    TimeSpan.FromMinutes(4),
                    TimeSpan.FromMinutes(5),
                    TimeSpan.FromMinutes(6),
                    TimeSpan.FromMinutes(7),
                    TimeSpan.FromMinutes(8),
                    TimeSpan.FromMinutes(9),
                    TimeSpan.FromMinutes(10),
                    TimeSpan.FromMinutes(11),
                    TimeSpan.FromMinutes(13),
                    TimeSpan.FromMinutes(15),
                    TimeSpan.FromMinutes(17),
                    TimeSpan.FromMinutes(19)));
        }

        private void LogAverageBreakAfterEventsStatistic(IFeedbackDatabase database, string fileName)
        {
            var calculator = new AverageBreakAfterEventsCalculator(5, TimeSpan.FromSeconds(30), true);

            var walker = new FeedbackProcessor(database, _logger);
            walker.Register(calculator);
            walker.ProcessFeedback();

            Output(fileName, calculator.StatisticAsCsv());
        }

        private void SelectConcurrentEvents(IFeedbackDatabase sourceDatabase, IFeedbackDatabase targetDatabase)
        {
            var filter = new FeedbackMapper(sourceDatabase, targetDatabase, _logger);
            filter.RegisterMapper(new AlwaysDropMapper());
            filter.RegisterMapper(new ToConcurrentEventMapper());
            filter.MapFeedback();
        }

        private void SelectCommandFollowupPairs(IFeedbackDatabase sourceDatabase,
            IFeedbackDatabase targetDatabase)
        {
            var filter = new FeedbackMapper(sourceDatabase, targetDatabase, _logger);
            filter.RegisterMapper(new CommandFollowupProcessor());
            filter.MapFeedback();
        }
    }
}
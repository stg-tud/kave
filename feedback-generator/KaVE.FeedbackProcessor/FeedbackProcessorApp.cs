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
 * 
 * Contributors:
 *    - Sven Amann
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    internal class FeedbackProcessorApp
    {
        private static readonly ILogger Logger = new ConsoleLogger();

        public static void Main()
        {
            const string importDatabase = "_import";
            const string filteredDatabase = "_filtered";
            const string cleanDatabase = "_clean";
            const string activityDatabase = "activities";
            const string concurrentEventDatabase = "_concurrent";
            const string commandFollowupsDatabase = "_commandFollowups";

            ImportFeedback(OpenDatabase(importDatabase));
            CleanFeedback(OpenDatabase(importDatabase), OpenDatabase(cleanDatabase));

            LogAnonymizationStatistics(OpenDatabase(importDatabase));
            LogEquivalentCommandPairs(OpenDatabase(filteredDatabase));
            LogDeveloperStatistics(OpenDatabase(importDatabase));
            LogEventsPerDeveloperDayStatistic(OpenDatabase(importDatabase));
            LogIDEActivationEvents(OpenDatabase(importDatabase));
            LogOccuringNames(OpenDatabase(importDatabase));

            MapToActivities(OpenDatabase(importDatabase), OpenDatabase(activityDatabase));
            LogActivityStatistics(OpenDatabase(activityDatabase));

            MapEquivalentCommands(OpenDatabase(filteredDatabase), OpenDatabase(cleanDatabase));

            MapToConcurrentEvents(OpenDatabase(filteredDatabase), OpenDatabase(concurrentEventDatabase));
            FilterCommandFollowupEvents(OpenDatabase(filteredDatabase), OpenDatabase(commandFollowupsDatabase));

            ConcurrentEventsStatistic(OpenDatabase(concurrentEventDatabase),"concurrenteventstatistic.csv");
            ConcurrentEventsStatistic(OpenDatabase(commandFollowupsDatabase),"commandfollowupsstatistic.csv");
        }

        private static MongoDbFeedbackDatabase OpenDatabase(string databaseSuffix)
        {
            return new MongoDbFeedbackDatabase(
                Configuration.DatabaseUrl,
                string.Format("{0}{1}", Configuration.DatasetName, databaseSuffix));
        }

        private static void Output(string outputFilename, string text)
        {
            File.WriteAllText(OutputFilename(outputFilename), text);
        }

        private static void Output(string outputFilename, IEnumerable<string> lines)
        {
            File.WriteAllLines(OutputFilename(outputFilename), lines);
        }

        private static string OutputFilename(string developerdaystatsCsv)
        {
            return Path.Combine(Configuration.StatisticsOutputPath, developerdaystatsCsv);
        }

        private static void ImportFeedback(IFeedbackDatabase importDatabase)
        {
            var feedbackImporter = new FeedbackImporter(importDatabase, Logger);
            feedbackImporter.Import();
        }

        public static void LogAnonymizationStatistics(IFeedbackDatabase database)
        {
            var calculator = new AnonymizationStatisticsLogger(database, Logger);
            calculator.LogNumberOfEventsWithoutSessionId();
            calculator.LogNumberOfEventsWithoutTriggerTime();
            calculator.LogNumberOfEventsWithoutDuration();
            calculator.LogNumberOfSessionsWithAnonymizedNames();
        }

        public static void LogDeveloperStatistics(IFeedbackDatabase database)
        {
            var stats = new DeveloperStatisticsCalculator(database);
            Logger.Info("We have at most {0} participant(s).", stats.GetUpperBoundToNumberOfParticipants());
            Logger.Info("We have at least {0} participant(s).", stats.GetLowerBoundToNumberOfParticipants());
            Logger.Info("We have {0} session(s) in total.", stats.GetNumberOfSessions());
            var conflicts = stats.GetNumberOfSessionsAssignedToMultipleDevelopers();
            if (conflicts > 0)
            {
                Logger.Error("We have {0} session(s) assigned to more than one developer!", conflicts);
            }
        }

        private static void LogEventsPerDeveloperDayStatistic(IFeedbackDatabase database)
        {
            var calculator1 = new EventsPerDeveloperDayStatisticCalculator(TimeSpan.FromMinutes(1));
            var calculator3 = new EventsPerDeveloperDayStatisticCalculator(TimeSpan.FromMinutes(3));
            var calculator5 = new EventsPerDeveloperDayStatisticCalculator(TimeSpan.FromMinutes(5));
            var calculator8 = new EventsPerDeveloperDayStatisticCalculator(TimeSpan.FromMinutes(8));
            var calculator13 = new EventsPerDeveloperDayStatisticCalculator(TimeSpan.FromMinutes(13));
            var calculator21 = new EventsPerDeveloperDayStatisticCalculator(TimeSpan.FromMinutes(21));
            var calculator34 = new EventsPerDeveloperDayStatisticCalculator(TimeSpan.FromMinutes(34));

            var walker = new FeedbackProcessor(database, Logger);
            walker.Register(calculator1);
            walker.Register(calculator3);
            walker.Register(calculator5);
            walker.Register(calculator8);
            walker.Register(calculator13);
            walker.Register(calculator21);
            walker.Register(calculator34);
            walker.ProcessFeedback();

            Output("developerdaystats-B1.csv", calculator1.StatisticAsCsv());
            Output("developerdaystats-B3.csv", calculator3.StatisticAsCsv());
            Output("developerdaystats-B5.csv", calculator5.StatisticAsCsv());
            Output("developerdaystats-B8.csv", calculator8.StatisticAsCsv());
            Output("developerdaystats-B13.csv", calculator13.StatisticAsCsv());
            Output("developerdaystats-B21.csv", calculator21.StatisticAsCsv());
            Output("developerdaystats-B34.csv", calculator34.StatisticAsCsv());
        }

        private static void LogOccuringNames(IFeedbackDatabase database)
        {
            var windowNameCollector = new WindowNameCollector();
            var documentNameCollector = new DocumentNameCollector();
            var commandIdCollector = new CommandIdCollector();

            var walker = new FeedbackProcessor(database, Logger);
            walker.Register(windowNameCollector);
            walker.Register(documentNameCollector);
            walker.Register(commandIdCollector);
            walker.ProcessFeedback();

            Output("windownames.log", windowNameCollector.AllWindowNames.EntryDictionary.ToCsv());
            Output("documentnames.log", documentNameCollector.AllDocumentNames.EntryDictionary.ToCsv());
            Output("commandids.log", commandIdCollector.AllCommandIds.EntryDictionary.ToCsv());
        }

        private static void LogIDEActivationEvents(IFeedbackDatabase database)
        {
            var calculator = new ParallelIDEInstancesStatisticCalculator();

            var walker = new FeedbackProcessor(database, Logger);
            walker.Register(calculator);
            walker.ProcessFeedback();

            Output("window-de-activation.log", calculator.Statistic);
        }

        private static void LogEquivalentCommandPairs(IFeedbackDatabase database)
        {
            var calculator = new EquivalentCommandPairCalculator();

            var walker = new FeedbackProcessor(database, Logger);
            walker.Register(calculator);
            walker.ProcessFeedback();

            Output("commandMappingsStatistic.csv", calculator.StatisticAsCsv());
        }

        private static void ConcurrentEventsStatistic(IFeedbackDatabase database, string fileName)
        {
            var calculator = new ConcurrentSetsCalculator();

            var walker = new FeedbackProcessor(database, Logger);
            walker.Register(calculator);
            walker.ProcessFeedback();

            Output(fileName, calculator.StatisticAsCsv());
        }

        private static void CleanFeedback(IFeedbackDatabase sourceDatabase, IFeedbackDatabase targetDatabase)
        {
            var cleaner = new FeedbackMapper(sourceDatabase, targetDatabase, Logger);
            cleaner.RegisterMapper(new AddFileProcessor());
            cleaner.RegisterMapper(new DuplicateCommandFilterProcessor()); ;
            cleaner.RegisterMapper(new EditFilterProcessor());
            cleaner.RegisterMapper(new ErrorFilterProcessor());
            cleaner.RegisterMapper(new UnnamedCommandFilterProcessor());
            cleaner.MapFeedback();
        }

        private static void MapToActivities(IFeedbackDatabase sourceDatabase, IFeedbackDatabase activityDatabase)
        {
            var activityMapper = new FeedbackMapper(sourceDatabase, activityDatabase, Logger);
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
            activityMapper.RegisterMapper(new WindowEventToActivityMapper(Logger));
            activityMapper.MapFeedback();
        }

        private static void LogActivityStatistics(IFeedbackDatabase activityDatabase)
        {
            var processor = new FeedbackProcessor(activityDatabase, Logger);
            var activityWindowProcessor = new ActivityWindowProcessor(new FrequencyActivityMergeStrategy(), TimeSpan.FromSeconds(1));
            processor.Register(activityWindowProcessor);
            processor.ProcessFeedback();

            var builder = new CsvBuilder();
            foreach (var developerWithStreams in activityWindowProcessor.ActivityStreams)
            {
                builder.StartRow();
                builder["Developer"] = developerWithStreams.Key.Id.ToString();
                builder["ActiveDays"] = developerWithStreams.Value.Count;
                foreach (var dayWithStream in developerWithStreams.Value)
                {
                    var statistics = dayWithStream.Value.Evaluate(TimeSpan.FromMinutes(10));
                    foreach (var activityWithDuration in statistics)
                    {
                        builder[dayWithStream.Key.ToString("yyyy-MM-dd") + " " + activityWithDuration.Key] = activityWithDuration.Value.TotalSeconds;
                    }
                }
            }
            builder.StartRow();
            foreach (var field in builder.Fields.Skip(2))
            {
                builder[field] = field.Split(' ')[0];
            }
            builder.StartRow();
            foreach (var field in builder.Fields.Skip(2))
            {
                builder[field] = field.Split(' ')[1];
            }
            Output("developer-activities.csv", builder.Build(CsvBuilder.SortFields.ByNameLeaveFirst));
        }

        private static void LogAverageBreakAfterEventsStatistic(IFeedbackDatabase database, string fileName)
        {
            var calculator = new AverageBreakAfterEventsCalculator();

            var walker = new FeedbackProcessor(database, Logger);
            walker.Register(calculator);
            walker.ProcessFeedback();

            Output(fileName, calculator.StatisticAsCsv());
        }

        private static void MapToConcurrentEvents(IFeedbackDatabase sourceDatabase, IFeedbackDatabase targetDatabase)
        {
            var filter = new FeedbackMapper(sourceDatabase, targetDatabase, Logger);
            filter.RegisterMapper(new AlwaysDropMapper());
            filter.RegisterMapper(new ToConcurrentEventMapper());
            filter.MapFeedback();
        }

        private static void MapEquivalentCommands(IFeedbackDatabase sourceDatabase, IFeedbackDatabase targetDatabase)
        {
            var filter = new FeedbackMapper(sourceDatabase, targetDatabase, Logger);
            filter.RegisterMapper(new MapEquivalentCommandsProcessor(new ResourceProvider()));
            filter.MapFeedback();
        }

        private static void FilterCommandFollowupEvents(IFeedbackDatabase sourceDatabase,
            IFeedbackDatabase targetDatabase)
        {
            var filter = new FeedbackMapper(sourceDatabase, targetDatabase, Logger);
            filter.RegisterMapper(new CommandFollowupProcessor());
            filter.MapFeedback();
        }
    }
}
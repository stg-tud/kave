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

using System.Collections.Generic;
using System.IO;
using KaVE.Commons.Utils.Exceptions;
using KaVE.FeedbackProcessor.Activities;
using KaVE.FeedbackProcessor.Cleanup.Processors;
using KaVE.FeedbackProcessor.Database;
using KaVE.FeedbackProcessor.Import;
using KaVE.FeedbackProcessor.Properties;
using KaVE.FeedbackProcessor.Statistics;

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
            const string equivalentCommandsDatabase = "_equivalentCommands";
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

            MapToConcurrentEvents(OpenDatabase(filteredDatabase), OpenDatabase(concurrentEventDatabase));
            FilterEquivalentCommandEvents(OpenDatabase(concurrentEventDatabase), OpenDatabase(equivalentCommandsDatabase));
            FilterCommandFollowupEvents(OpenDatabase(filteredDatabase), OpenDatabase(commandFollowupsDatabase));

            ConcurrentEventsStatistic(OpenDatabase(concurrentEventDatabase),"concurrenteventstatistic.csv");
            ConcurrentEventsStatistic(OpenDatabase(equivalentCommandsDatabase),"equivalentcommandstatistic.csv");
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
            var calculator = new EventsPerDeveloperDayStatisticCalculator();

            var walker = new FeedbackProcessor(database, Logger);
            walker.Register(calculator);
            walker.ProcessFeedback();

            Output("developerdaystats.csv", calculator.StatisticAsCsv());
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

            Output("windownames.log", windowNameCollector.AllWindowNames);
            Output("documentnames.log", documentNameCollector.AllDocumentNames);
            Output("commandids.log", commandIdCollector.AllCommandIds);
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
            var cleaner = new FeedbackMapper(sourceDatabase, targetDatabase);
            cleaner.RegisterMapper<AddFileProcessor>();
            cleaner.RegisterMapper<DuplicateCommandFilterProcessor>(); ;
            cleaner.RegisterMapper<EditFilterProcessor>();
            cleaner.RegisterMapper<ErrorFilterProcessor>();
            cleaner.RegisterMapper<UnnamedCommandFilterProcessor>();
            cleaner.MapFeedback();
        }

        private static void MapToActivities(IFeedbackDatabase sourceDatabase, IFeedbackDatabase activityDatabase)
        {
            var activityMapper = new FeedbackMapper(sourceDatabase, activityDatabase);
            activityMapper.RegisterMapper<AlwaysDropMapper>(); // only generated events reach activity database
            activityMapper.RegisterMapper<AnyToActivityMapper>(); // map any event to a keep-alive
            activityMapper.RegisterMapper<BuildEventToActivityMapper>();
            activityMapper.RegisterMapper<CommandEventToActivityMapper>();
            activityMapper.RegisterMapper<CompletionEventToActivityMapper>();
            activityMapper.RegisterMapper<DebuggerEventToActivityMapper>();
            activityMapper.RegisterMapper<DocumentEventToActivityMapper>();
            activityMapper.RegisterMapper<EditEventToActivityMapper>();
            activityMapper.RegisterMapper<FindEventToActivityMapper>();
            activityMapper.RegisterMapper<IDEStateEventToActivityMapper>();
            activityMapper.RegisterMapper<InIDEToActivityDetector>();
            activityMapper.RegisterMapper<InstallEventToActivityMapper>();
            activityMapper.RegisterMapper<SolutionEventToActivityMapper>();
            activityMapper.RegisterMapper<UpdateEventToActivityMapper>();
            //activityMapper.RegisterMapper<WindowEventToActivityMapper>();
            activityMapper.MapFeedback();
        }

        private static void MapToConcurrentEvents(IFeedbackDatabase sourceDatabase, IFeedbackDatabase targetDatabase)
        {
            var filter = new FeedbackMapper(sourceDatabase, targetDatabase);
            filter.RegisterMapper<AlwaysDropMapper>();
            filter.RegisterMapper<ToConcurrentEventMapper>();
            filter.MapFeedback();
        }

        private static void FilterEquivalentCommandEvents(IFeedbackDatabase sourceDatabase,
            IFeedbackDatabase targetDatabase)
        {
            var filter = new FeedbackMapper(sourceDatabase, targetDatabase);
            filter.RegisterMapper<EquivalentCommandProcessor>();
            filter.MapFeedback();
        }

        private static void FilterCommandFollowupEvents(IFeedbackDatabase sourceDatabase,
            IFeedbackDatabase targetDatabase)
        {
            var filter = new FeedbackMapper(sourceDatabase, targetDatabase);
            filter.RegisterMapper<CommandFollowupProcessor>();
            filter.MapFeedback();
        }
    }
}
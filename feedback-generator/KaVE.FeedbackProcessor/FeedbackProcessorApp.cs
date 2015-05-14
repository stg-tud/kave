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

using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Exceptions;
using KaVE.FeedbackProcessor.Activities;
using KaVE.FeedbackProcessor.Cleanup;
using KaVE.FeedbackProcessor.Cleanup.Processors;
using KaVE.FeedbackProcessor.Database;
using KaVE.FeedbackProcessor.Import;
using KaVE.FeedbackProcessor.Properties;
using KaVE.FeedbackProcessor.Statistics;
using MongoDB.Driver.Linq;

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
            const string mergedCommandsDatabase = "_mergedCommands";

            //ImportFeedback(OpenDatabase(importDatabase));

            //LogDeveloperStatistics(OpenDatabase(importDatabase));
            //LogAnonymizationStatistics(OpenDatabase(importDatabase));
            //ComputeEventsPerDeveloperDayStatistic(OpenDatabase(importDatabase));
            //CollectNames(OpenDatabase(importDatabase));
            LogIDEActivationEvents(OpenDatabase(importDatabase));

            //CleanImportDatabase(OpenDatabase(importDatabase), OpenDatabase(filteredDatabase));

            //MapToActivities(OpenDatabase(importDatabase), OpenDatabase(activityDatabase));
            CommandMappingsStatistic(OpenDatabase(filteredDatabase));

            //CleanFeedback(OpenDatabase(filteredDatabase), OpenDatabase(cleanDatabase));

            //FilterConcurrentEvents(OpenDatabase(filteredDatabase), OpenDatabase(concurrentEventDatabase));

            //FilterEquivalentCommandEvents(OpenDatabase(concurrentEventDatabase), OpenDatabase(equivalentCommandsDatabase));

            //FilterCommandFollowupEvents(OpenDatabase(filteredDatabase), OpenDatabase(commandFollowupsDatabase));

            //ConcurrentEventsStatistic(OpenDatabase(concurrentEventDatabase),"concurrenteventstatistic.csv");

            //ConcurrentEventsStatistic(OpenDatabase(equivalentCommandsDatabase),"equivalentcommandstatistic.csv");

            //ConcurrentEventsStatistic(OpenDatabase(commandFollowupsDatabase),"commandfollowupsstatistic.csv");
        }

        private static MongoDbFeedbackDatabase OpenDatabase(string databaseSuffix)
        {
            return new MongoDbFeedbackDatabase(
                Configuration.DatabaseUrl,
                string.Format("{0}{1}", Configuration.DatasetName, databaseSuffix));
        }

        private static void ImportFeedback(IFeedbackDatabase importDatabase)
        {
            var feedbackImporter = new FeedbackImporter(importDatabase, Logger);
            feedbackImporter.Import();
        }

        public static void LogAnonymizationStatistics(IFeedbackDatabase database)
        {
            var calculator = new AnonymizationStatisticsCalculator(database, Logger);
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

        private static void ComputeEventsPerDeveloperDayStatistic(IFeedbackDatabase database)
        {
            var walker = new FeedbackWalker(database, Logger);
            var calculator = new EventsPerDeveloperDayStatisticCalculator();
            walker.Register(calculator);
            walker.ProcessFeedback();

            var statistic = calculator.Statistic;
            var csvBuilder = new CsvBuilder();
            foreach (var stat in statistic)
            {
                csvBuilder.StartRow();
                csvBuilder["Developer"] = stat.Key.Id;
                foreach (var day in stat.Value)
                {
                    var dayString = day.Day.ToString("yyyy-MM-dd");
                    csvBuilder[dayString + " 1 Start"] = day.FirstActivityAt;
                    csvBuilder[dayString + " 2 End"] = day.LastActivityAt;
                    csvBuilder[dayString + " 3 Events"] = day.NumberOfEvents;
                    csvBuilder[dayString + " 4 Breaks"] = day.NumberOfBreaks;
                    csvBuilder[dayString + " 5 Total Break"] = day.TotalBreakTime;
                }
            }
            File.WriteAllText(
                Path.Combine(Configuration.StatisticsOutputPath, "developerdaystats.csv"),
                csvBuilder.Build(CsvBuilder.SortFields.ByNameLeaveFirst));
        }

        private static void CollectNames(IFeedbackDatabase database)
        {
            var walker = new FeedbackWalker(database, Logger);
            var windowNameCollector = new WindowNameCollector();
            walker.Register(windowNameCollector);
            var documentNameCollector = new DocumentNameCollector();
            walker.Register(documentNameCollector);
            var commandIdCollector = new CommandIdCollector();
            walker.Register(commandIdCollector);

            walker.ProcessFeedback();

            File.WriteAllLines(
                Path.Combine(Configuration.StatisticsOutputPath, "windownames.log"),
                windowNameCollector.AllWindowNames.Select(wn => wn.Identifier));
            File.WriteAllLines(
                Path.Combine(Configuration.StatisticsOutputPath, "documentnames.log"),
                documentNameCollector.AllDocumentNames.Select(dn => dn.Identifier));
            File.WriteAllLines(
                Path.Combine(Configuration.StatisticsOutputPath, "commandids.log"),
                commandIdCollector.AllCommandIds);
        }

        private static void LogIDEActivationEvents(IFeedbackDatabase database)
        {
            var walker = new FeedbackWalker(database, Logger);
            walker.Register(new ParallelIDEInstancesStatisticCalculator(new FileLogger(Path.Combine(Configuration.StatisticsOutputPath, "window-de-activation.log"))));
            walker.ProcessFeedback();
        }

        private static void CommandMappingsStatistic(IFeedbackDatabase database)
        {
            var walker = new FeedbackWalker(database, Logger);
            var calculator = new CommandMappingsCalculator();

            walker.Register(calculator);
            walker.ProcessFeedback();

            var statistic = CommandMappingsCalculator.Statistic.OrderByDescending(keyValuePair => keyValuePair.Value);

            var csvBuilder = new CsvBuilder();

            foreach (var stat in statistic)
            {
                csvBuilder.StartRow();

                csvBuilder["FirstCommand"] = stat.Key.Item1;
                csvBuilder["SecondCommand"] = stat.Key.Item2;
                csvBuilder["Count"] = stat.Value;
            }

            File.WriteAllText(
                Path.Combine(Configuration.StatisticsOutputPath, "commandMappingsStatistic.csv"),
                csvBuilder.Build());
        }

        private static void ConcurrentEventsStatistic(IFeedbackDatabase database, string fileName)
        {
            var walker = new FeedbackWalker(database, Logger);
            var calculator = new ConcurrentSetsCalculator();

            walker.Register(calculator);
            walker.ProcessFeedback();

            var statistic = calculator.Statistic.OrderByDescending(keyValuePair => keyValuePair.Value);
            var csvBuilder = new CsvBuilder();

            var maximumNumberOfEventFields = statistic.Max(stat => stat.Key.Count);

            foreach (var stat in statistic)
            {
                csvBuilder.StartRow();

                var eventList = stat.Key.ToList();

                for (int i = 0; i < maximumNumberOfEventFields; i++)
                {
                    var fieldName = "Event" + i;
                    if (i < eventList.Count) csvBuilder[fieldName] = eventList[i];
                    else csvBuilder[fieldName] = "";
                }

                csvBuilder["Count"] = stat.Value;
            }

            File.WriteAllText(
                Path.Combine(Configuration.StatisticsOutputPath, fileName),
                csvBuilder.Build());
        }

        private static void CleanFeedback(IFeedbackDatabase sourceDatabase, IFeedbackDatabase targetDatabase)
        {
            var cleaner = new EventsMapper(sourceDatabase, targetDatabase);
            cleaner.RegisterProcessor<AddFileProcessor>();
            cleaner.RegisterProcessor<DuplicateCommandFilterProcessor>();
            cleaner.RegisterProcessor<ErrorFilterProcessor>();
            cleaner.RegisterProcessor<UnnamedCommandFilterProcessor>();
            cleaner.ProcessFeedback();
        }

        private static void MapToActivities(IFeedbackDatabase sourceDatabase, IFeedbackDatabase activityDatabase)
        {
            var activityMapper = new EventsMapper(sourceDatabase, activityDatabase);
            activityMapper.RegisterProcessor<AlwaysDropProcessor>(); // only generated events reach activity database
            activityMapper.RegisterProcessor<AnyActivityActivityProcessor>(); // map any event to a keep-alive
            activityMapper.RegisterProcessor<InIDEActivityDetector>();
            activityMapper.RegisterProcessor<IDEStateEventActivityProcessor>();
            activityMapper.RegisterProcessor<BuildEventActivityProcessor>();
            activityMapper.RegisterProcessor<DebuggerEventActivityProcessor>();

            activityMapper.ProcessFeedback();
        }
        
        private static void FilterConcurrentEvents(IFeedbackDatabase sourceDatabase, IFeedbackDatabase targetDatabase)
        {
            var filter = new EventsMapper(sourceDatabase, targetDatabase);
            filter.RegisterProcessor<ConcurrentEventProcessor>();
            filter.ProcessFeedback();
        }

        private static void FilterEquivalentCommandEvents(IFeedbackDatabase sourceDatabase,
            IFeedbackDatabase targetDatabase)
        {
            var filter = new EventsMapper(sourceDatabase, targetDatabase);
            filter.RegisterProcessor<EquivalentCommandProcessor>();
            filter.ProcessFeedback();
        }

        private static void FilterCommandFollowupEvents(IFeedbackDatabase sourceDatabase, IFeedbackDatabase targetDatabase)
        {
            var filter = new EventsMapper(sourceDatabase, targetDatabase);
            filter.RegisterProcessor<CommandFollowupProcessor>();
            filter.ProcessFeedback();
        }

        private static void CleanImportDatabase(IFeedbackDatabase sourceDatabase, IFeedbackDatabase targetDatabase)
        {
            var filter = new EventsMapper(sourceDatabase, targetDatabase);
            filter.RegisterProcessor<ErrorFilterProcessor>();
            filter.RegisterProcessor<EditFilterProcessor>();
            filter.RegisterProcessor<UnnamedCommandFilterProcessor>();
            filter.RegisterProcessor<DuplicateCommandFilterProcessor>();
            filter.ProcessFeedback();
        }
    }
}
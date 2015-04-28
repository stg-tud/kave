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
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Exceptions;
using KaVE.FeedbackProcessor.Cleanup;
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
            const string importDatabase = "";
            const string cleanDatabase = "_clean";

            //ImportFeedback(OpenDatabase(importDatabase));

            //LogDeveloperStatistics(OpenDatabase(importDatabase));
            //LogAnonymizationStatistics(OpenDatabase(importDatabase));
            ComputeEventsPerDeveloperDayStatistic(OpenDatabase(importDatabase));

            //CleanFeedback(OpenDatabase(importDatabase), OpenDatabase(cleanDatabase));
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
                    csvBuilder[day.Date.ToString("MM/dd/yyyy")] = day.NumberOfEvents;
                }
            }
            File.WriteAllText(Path.Combine(Configuration.StatisticsOutputPath, "developerdaystats.csv"), csvBuilder.Build());
        }

        private static void CleanFeedback(IFeedbackDatabase sourceDatabase, IFeedbackDatabase targetDatabase)
        {
            var cleaner = new EventsMapper(sourceDatabase, targetDatabase);
            cleaner.ProcessFeedback();
        }
    }
}
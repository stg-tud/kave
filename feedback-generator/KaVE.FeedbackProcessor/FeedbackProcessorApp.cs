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
            var database = new MongoDbFeedbackDatabase(Configuration.DatabaseUrl, Configuration.DatabaseName);
            //ImportFeedback(database);
            //LogStatistics(database);
            //CleanFeedback(database);
        }

        private static void ImportFeedback(IFeedbackDatabase database)
        {
            var feedbackImporter = new FeedbackImporter(database, Logger);
            feedbackImporter.Import();
        }

        private static void LogStatistics(IFeedbackDatabase database)
        {
            LogDeveloperStatistics(database);

            var calculator = new AnonymizationStatisticsCalculator(database, Logger);
            calculator.LogNumberOfEventsWithoutSessionId();
            calculator.LogNumberOfEventsWithoutTriggerTime();
            calculator.LogNumberOfEventsWithoutDuration();
            calculator.LogNumberOfSessionsWithAnonymizedNames();
        }

        private static void LogDeveloperStatistics(IFeedbackDatabase database)
        {
            var stats = new DeveloperStatisticsCalculator(database);
            Logger.Info(string.Format("We have at most {0} participant(s).", stats.GetUpperBoundToNumberOfParticipants()));
            Logger.Info(string.Format("We have at least {0} participant(s).", stats.GetLowerBoundToNumberOfParticipants()));
            Logger.Info(string.Format("We have {0} session(s) in total.", stats.GetNumberOfSessions()));
            var conflicts = stats.GetNumberOfSessionsAssignedToMultipleDevelopers();
            if (conflicts > 0)
            {
                Logger.Error(string.Format("We have {0} session(s) assigned to more than one developer!", conflicts));
            }
        }

        private static void CleanFeedback(IFeedbackDatabase database)
        {
            var cleaner = new FeedbackCleaner(database);
            cleaner.ProcessFeedback();
        }
    }
}
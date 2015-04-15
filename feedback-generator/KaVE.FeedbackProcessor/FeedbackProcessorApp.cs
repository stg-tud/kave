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
using KaVE.FeedbackProcessor.Database;

namespace KaVE.FeedbackProcessor
{
    internal class FeedbackProcessorApp
    {
        private static readonly ILogger Logger = new ConsoleLogger();

        public static void Main()
        {
            var database = new FeedbackDatabase(Configuration.DatabaseUrl, Configuration.DatabaseName);
            ImportFeedback(database);
            //CleanFeedback(database);
        }

        private static void ImportFeedback(FeedbackDatabase database)
        {
            var feedbackImporter = new FeedbackImporter(database, Logger);
            feedbackImporter.Import();
            feedbackImporter.LogDeveloperStatistics();
        }

        private static void CleanFeedback(FeedbackDatabase database)
        {
            var cleaner = new FeedbackCleaner(database);
            cleaner.IterateEventsPerDeveloper();
        }
    }
}

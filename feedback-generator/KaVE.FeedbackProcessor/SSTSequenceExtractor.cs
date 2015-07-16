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
using System.Linq;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Utils.Exceptions;
using KaVE.Commons.Utils.ObjectUsageExport;
using KaVE.FeedbackProcessor.Database;
using KaVE.FeedbackProcessor.Database.DevelopmentHistory;
using KaVE.FeedbackProcessor.Properties;

namespace KaVE.FeedbackProcessor
{
    internal class SSTSequenceExtractor
    {
        private readonly ILogger _logger;

        public SSTSequenceExtractor(ILogger logger)
        {
            _logger = logger;
        }

        private IFeedbackDatabase OpenDatabase(string databaseSuffix)
        {
            return MongoDbFeedbackDatabase.Open(Configuration.DatabaseUrl, Configuration.DatasetName, databaseSuffix);
        }

        public void Run()
        {
            using (var db = new DevelopmentHistoryDatabase(Configuration.StatisticsOutputPath + "\\test-database"))
            {
                var feedbackDatabase = OpenDatabase("_import");
                var developerCollection = feedbackDatabase.GetDeveloperCollection();
                var eventsCollection = feedbackDatabase.GetEventsCollection();
                foreach (var developer in developerCollection.FindAll())
                {
                    var workPeriod = developer.Id.ToString();
                    _logger.Info("Processing WorkPeriod {0}...", workPeriod);
                    foreach (
                        var completionEvent in
                            eventsCollection.GetEventStream(developer).OfType<CompletionEvent>())
                    {
                        var timestamp = completionEvent.TriggeredAt.GetValueOrDefault();
                        var ctx = completionEvent.Context2;

                        Insert(db, workPeriod, timestamp, ctx);
                    }
                }
            }
        }

        private static void Insert(DevelopmentHistoryDatabase db, string workPeriod, DateTime timestamp, Context ctx)
        {
            db.Insert(workPeriod, timestamp, ctx);

            var usageExtractor = new UsageExtractor();
            var usages = usageExtractor.Export(ctx);

            var queryExtractor = new QueryExtractor();
            var query = queryExtractor.Extract(ctx);

            foreach (var usage in usages)
            {
                var isQuery = usage.Equals(query);
                db.Insert(workPeriod, timestamp, usage, isQuery);
            }
        }
    }
}
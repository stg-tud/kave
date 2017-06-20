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
using System.Linq;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
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
        private readonly UsageExtractor _usageExtractor = new UsageExtractor();
        private readonly QueryExtractor _queryExtractor = new QueryExtractor();

        private int _numberOfSSTs;
        private int _numberOfOUs;
        private readonly ISet<string> _errorMessages = new HashSet<string>();

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
            using (var db = new DevelopmentHistoryDatabase("G:\\DATEV-269-histories.sqlite"))
            {
                var feedbackDatabase = OpenDatabase("_import");
                var developerCollection = feedbackDatabase.GetDeveloperCollection();
                var eventsCollection = feedbackDatabase.GetEventsCollection();
                foreach (var developer in developerCollection.FindAll())
                {
                    var workPeriod = developer.Id.ToString();
                    _numberOfSSTs = 0;
                    _numberOfOUs = 0;
                    _errorMessages.Clear();
                    _logger.Info("Processing WorkPeriod {0}...", workPeriod);
                    foreach (
                        var completionEvent in
                            eventsCollection.GetEventStream(developer).OfType<CompletionEvent>())
                    {
                        var timestamp = completionEvent.TriggeredAt.GetValueOrDefault();
                        var ctx = completionEvent.Context2;

                        Insert(db, workPeriod, timestamp, ctx);
                    }
                    _logger.Info(" > Inserted {0} SSTs and {1} OUs.", _numberOfSSTs, _numberOfOUs);
                    if (_errorMessages.Any())
                    {
                        _logger.Info(
                            " > {0} errors occured:\n     - {1}",
                            _errorMessages.Count,
                            string.Join("\n     - ", _errorMessages));
                    }
                }
            }
        }

        private void Insert(DevelopmentHistoryDatabase db, string workPeriod, DateTime timestamp, Context ctx)
        {
            var query = GetQuery(ctx);
            var usages = GetObjectUsages(ctx);

            if (!usages.Any())
            {
                // No need to blow up the dataset with SST that don't contain any code. Most likely our
                // analysis failed to extract the context in these cases.
                return;
            }

            db.Insert(workPeriod, timestamp, ctx, query);
            _numberOfSSTs++;

            foreach (var usage in usages)
            {
                var isQuery = usage.Equals(query);
                db.Insert(workPeriod, timestamp, usage, isQuery);
                _numberOfOUs++;
            }
        }

        private IKaVEList<Query> GetObjectUsages(Context ctx)
        {
            try
            {
                return _usageExtractor.Export(ctx);
            }
            catch (AssertException ae)
            {
                // happens for anonymized names, for now, I just ignore these contexts
                _errorMessages.Add(ae.Message);
                return Lists.NewList<Query>();
            }
        }

        private Query GetQuery(Context ctx)
        {
            try
            {
                return _queryExtractor.Extract(ctx) ?? new Query();
            }
            catch (AssertException ae)
            {
                // happens for anonymized names, for now, I just ignore these contexts
                _errorMessages.Add(ae.Message);
                return new Query();
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return new Query();
            }
        }
    }
}
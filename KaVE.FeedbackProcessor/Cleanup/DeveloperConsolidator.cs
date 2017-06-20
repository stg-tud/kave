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

using System.Linq;
using KaVE.Commons.Utils.Exceptions;
using KaVE.FeedbackProcessor.Database;
using KaVE.FeedbackProcessor.Model;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

namespace KaVE.FeedbackProcessor.Cleanup
{
    class DeveloperConsolidator
    {
        private readonly IFeedbackDatabase _database;
        private readonly ILogger _logger;

        public DeveloperConsolidator(IFeedbackDatabase database, ILogger logger)
        {
            _database = database;
            _logger = logger;
        }

        public void ConsolidateDevelopers()
        {
            var dupId = new ObjectId(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            var developerCollection = _database.GetDeveloperCollection();
            var devs = developerCollection.FindAll().ToList();
            var consolidatedSomeDevelopers = false;
            foreach (var dev in devs)
            {
                if (dev.Id == dupId) continue;
                var duplicates = devs.Where(otherDev => !dev.Id.Equals(otherDev.Id) && dev.SessionIds.Overlaps(otherDev.SessionIds)).ToList();

                if (duplicates.Count > 0)
                {
                    _logger.Info("Found {0} duplicates of developer {1}, consolidating...", duplicates.Count, dev.Id);
                    consolidatedSomeDevelopers = true;
                    foreach (var duplicate in duplicates)
                    {
                        _logger.Info(" - Merging developer {0} with {1} sessions.", duplicate.Id, duplicate.SessionIds.Count);
                        foreach (var sessionId in duplicate.SessionIds)
                        {
                            dev.SessionIds.Add(sessionId);
                        }
                        ((MongoDbDeveloperCollection)developerCollection).Collection.Remove(
                            Query<Developer>.EQ(d => d.Id, duplicate.Id));
                        duplicate.Id = dupId;
                    }
                    developerCollection.Save(dev);
                }
            }
            // Fixpoint iteration, consolidate until there's no conflicts left
            if (consolidatedSomeDevelopers)
            {
                ConsolidateDevelopers();
            }
        }
    }
}

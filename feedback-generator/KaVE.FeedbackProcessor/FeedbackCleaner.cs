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
using KaVE.Commons.Model.Events;
using KaVE.FeedbackProcessor.Database;
using MongoDB.Driver.Builders;

namespace KaVE.FeedbackProcessor
{
    internal class FeedbackCleaner
    {
        private readonly FeedbackDatabase _database;

        private static void Main()
        {
            var database = new FeedbackDatabase(Configuration.DatabaseUrl, Configuration.DatabaseName);
            var cleaner = new FeedbackCleaner(database);
            cleaner.IterateEventsPerDeveloper();
        }

        private FeedbackCleaner(FeedbackDatabase database)
        {
            _database = database;
        }

        private void IterateEventsPerDeveloper()
        {
            var developers = _database.GetDeveloperCollection();

            foreach (var developer in developers.FindAll())
            {
                foreach (var ideEvent in GetAllEventsOf(developer))
                {
                    // do something with the events
                }
            }
        }

        private IEnumerable<IDEEvent> GetAllEventsOf(Developer developer)
        {
            var events = _database.GetEventsCollection();
            return
                events.Find(Query<IDEEvent>.In(evt => evt.IDESessionUUID, developer.SessionIds))
                      .SetSortOrder(SortBy<IDEEvent>.Ascending(evt => evt.TriggeredAt));
        }
    }
}
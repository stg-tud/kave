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
using System.Linq;
using KaVE.Commons.Model.Events;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace KaVE.FeedbackProcessor.Database
{
    internal class MongoDbIDEEventCollection : MongoDbDatabaseCollection<IDEEvent>, IIDEEventCollection
    {
        public MongoDbIDEEventCollection(MongoCollection<IDEEvent> collection) : base(collection) {}

        public IEnumerable<IDEEvent> GetEventStream(Developer developer)
        {
            return Collection.Find(Query<IDEEvent>.In(evt => evt.IDESessionUUID, developer.SessionIds))
                             .SetSortOrder(SortBy<IDEEvent>.Ascending(evt => evt.TriggeredAt));
        }

        public bool Contains(IDEEvent @event)
        {
            var candidates = Collection.FindAs<IDEEvent>(
                Query.And(
                    Query<IDEEvent>.EQ(e => e.IDESessionUUID, @event.IDESessionUUID),
                    Query<IDEEvent>.EQ(e => e.TriggeredAt, @event.TriggeredAt),
                    Query.EQ("_t", @event.GetType().Name)));
            return candidates.Any(@event.Equals);
        }
    }
}
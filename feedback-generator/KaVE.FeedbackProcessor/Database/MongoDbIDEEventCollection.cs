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

using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Reflection;
using KaVE.FeedbackProcessor.Model;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace KaVE.FeedbackProcessor.Database
{
    internal class MongoDbIDEEventCollection : MongoDbDatabaseCollection<IDEEvent>, IIDEEventCollection
    {
        public MongoDbIDEEventCollection(MongoCollection<IDEEvent> collection) : base(collection)
        {
            EnsureEventIdentityIndex();
            EnsureEventChronologicalIndex();
        }

        /// <summary>
        ///     Events are almost unique with repsect to their session id, trigger time, and type. We need this index to enable
        ///     performant Contains checks.
        /// </summary>
        private void EnsureEventIdentityIndex()
        {
            var evtIndex = IndexKeys.Ascending(TypeExtensions<IDEEvent>.GetPropertyName(evt => evt.IDESessionUUID))
                                    .Ascending(TypeExtensions<IDEEvent>.GetPropertyName(evt => evt.TriggeredAt))
                                    .Ascending("_t");
            var options = IndexOptions<IDEEvent>.SetUnique(false).SetSparse(true);
            EnsureIndex(evtIndex, options);
        }

        /// <summary>
        ///     Sorting by trigger time when retrieving a developer's event stream needs this index to be performant.
        /// </summary>
        private void EnsureEventChronologicalIndex()
        {
            var evtIndex = IndexKeys.Ascending(TypeExtensions<IDEEvent>.GetPropertyName(evt => evt.TriggeredAt));
            var options = IndexOptions<IDEEvent>.SetUnique(false).SetSparse(false);
            EnsureIndex(evtIndex, options);
        }

        private void EnsureIndex(IMongoIndexKeys evtIndex, IMongoIndexOptions options)
        {
            if (!Collection.IndexExists(evtIndex))
            {
                Collection.CreateIndex(evtIndex, options);
            }
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

        public IEnumerable<IDEEvent> Find(IMongoQuery query)
        {
            return Collection.Find(query);
        }

        public IEnumerable<T> FindAs<T>(IMongoQuery query) where T : IDEEvent
        {
            return Collection.FindAs<T>(query);
        }

        public IEnumerable<IDEEvent> GetEventStream(Developer developer)
        {
            return Collection.Find(EventsFrom(developer)).SetSortOrder(Chronological);
        }

        private static SortByBuilder<IDEEvent> Chronological
        {
            get { return SortBy<IDEEvent>.Ascending(evt => evt.TriggeredAt); }
        }

        private static IMongoQuery EventsFrom(Developer developer)
        {
            return Query<IDEEvent>.In(evt => evt.IDESessionUUID, developer.SessionIds);
        }
    }
}
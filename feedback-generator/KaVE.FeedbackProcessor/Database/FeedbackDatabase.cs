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

using System;
using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Reflection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace KaVE.FeedbackProcessor.Database
{
    {

        {
        }
    }

    internal class FeedbackDatabase : IFeedbackDatabase
    {
        private readonly MongoDatabase _database;

        public FeedbackDatabase(string databaseUrl, string databaseName)
        {
            var client = new MongoClient(databaseUrl);
            var server = client.GetServer();
            _database = server.GetDatabase(databaseName);
            RegisterModel();
        }

        public IDeveloperCollection GetDeveloperCollection()
        {
            return new MongoDbDeveloperCollection(GetCollection<Developer>());
        }

        public MongoCollection<IDEEvent> GetEventsCollection()
        {
            var eventsCollection = GetCollection<IDEEvent>();
            EnsureEventIndex(eventsCollection);
            return eventsCollection;
        }

        private MongoCollection<T> GetCollection<T>()
        {
            var collectionName = typeof (T).Name;
            if (!_database.CollectionExists(collectionName))
            {
                _database.CreateCollection(collectionName);
            }
            return _database.GetCollection<T>(collectionName);
        }

        private static void EnsureEventIndex(MongoCollection<IDEEvent> eventsCollection)
        {
            var evtIndex = IndexKeys.Ascending(TypeExtensions<IDEEvent>.GetPropertyName(evt => evt.IDESessionUUID))
                                    .Ascending(TypeExtensions<IDEEvent>.GetPropertyName(evt => evt.TriggeredAt))
                                    .Ascending("_t");
            if (!eventsCollection.IndexExists(evtIndex))
            {
                eventsCollection.CreateIndex(
                    evtIndex,
                    IndexOptions<IDEEvent>.SetUnique(false).SetSparse(true));
            }
        }

        private static void RegisterModel()
        {
            BsonSerializer.RegisterSerializer(typeof (DateTime), new DateTimeTicksSerializer());
            BsonSerializer.RegisterGenericSerializerDefinition(typeof (IKaVESet<>), typeof (KaVECollectionSerializer<>));
            BsonSerializer.RegisterGenericSerializerDefinition(
                typeof (IKaVEList<>),
                typeof (KaVECollectionSerializer<>));
            BsonSerializer.RegisterSerializer(typeof (IProposalCollection), new KaVECollectionSerializer());
            BsonClassMap.RegisterClassMap<IDEEvent>(
                cm =>
                {
                    cm.AutoMap();
                    cm.SetIdMember(cm.GetMemberMap(c => c.Id));
                    cm.IdMemberMap.SetIdGenerator(StringObjectIdGenerator.Instance);
                });
            foreach (var type in GetAllModelTypes())
            {
                BsonClassMap.LookupClassMap(type);
            }
        }

        private static IEnumerable<Type> GetAllModelTypes()
        {
            return typeof (IDEEvent).Assembly.GetTypes().Where(IsModelClass);
        }

        private static bool IsModelClass(Type t)
        {
            return t.IsClass && !t.IsAbstract && !t.ContainsGenericParameters &&
                   t.Namespace != null && t.Namespace.StartsWith("KaVE.Commons.Model");
        }
    }
}
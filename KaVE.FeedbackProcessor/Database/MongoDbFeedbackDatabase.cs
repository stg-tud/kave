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
using System.Reflection;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Model;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;

namespace KaVE.FeedbackProcessor.Database
{
    internal class MongoDbFeedbackDatabase : IFeedbackDatabase
    {
        public static IFeedbackDatabase Open(string url, string baseName, string nameSuffix)
        {
            return new MongoDbFeedbackDatabase(url, string.Format("{0}{1}", baseName, nameSuffix));
        }

        private readonly MongoDatabase _database;

        public MongoDbFeedbackDatabase(string databaseUrl, string databaseName)
        {
            var client = new MongoClient(databaseUrl);
            var server = client.GetServer();
            _database = server.GetDatabase(databaseName);
        }

        static MongoDbFeedbackDatabase()
        {
            RegisterModel();
        }

        public IDeveloperCollection GetDeveloperCollection()
        {
            return new MongoDbDeveloperCollection(GetCollection<Developer>());
        }

        public IIDEEventCollection GetEventsCollection()
        {
            var eventsCollection = GetCollection<IDEEvent>();
            return new MongoDbIDEEventCollection(eventsCollection);
        }

        private MongoCollection<T> GetCollection<T>(String suffix = "")
        {
            var collectionName = typeof (T).Name + suffix;
            if (!_database.CollectionExists(collectionName))
            {
                _database.CreateCollection(collectionName);
            }
            return _database.GetCollection<T>(collectionName);
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
            RegisterModelTypesFrom(typeof (IDEEvent).Assembly);
            RegisterModelTypesFrom(typeof (MongoDbFeedbackDatabase).Assembly);
            BsonClassMap.LookupClassMap(typeof (ConcurrentEvent));
        }

        private static void RegisterModelTypesFrom(Assembly assembly)
        {
            foreach (var type in GetAllModelTypes(assembly))
            {
                BsonClassMap.LookupClassMap(type);
            }
        }

        private static IEnumerable<Type> GetAllModelTypes(Assembly assembly)
        {
            return assembly.GetTypes().Where(IsModelClass);
        }

        private static bool IsModelClass(Type t)
        {
            return t.IsClass && !t.IsAbstract && !t.ContainsGenericParameters &&
                   t.Namespace != null && t.Namespace.Contains(".Model");
        }
    }
}
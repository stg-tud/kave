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
using System.IO;
using System.Linq;
using Ionic.Zip;
using KaVE.Commons.Model.Events;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace KaVE.FeedbackProcessor
{
    internal class FeedbackImporter
    {
        private const string ImportDirectory = @"C:\Users\Sven\Documents\KaVE\Feedback\kave.st";

        private const string DatabaseUrl = "mongodb://localhost";
        private const string DatabaseName = "local";

        private static void Main(string[] args)
        {
            var database = GetDatabase();
            var eventsCollection = GetCollection<IDEEvent>(database);
            var developerCollection = GetCollection<Developer>(database);

            Import(developerCollection, eventsCollection);
        }

        private static void Import(MongoCollection<Developer> developerCollection, MongoCollection<IDEEvent> eventsCollection)
        {
            var fileLoader = new FileLoader();
            foreach (
                var archive in
                    Directory.GetFiles(ImportDirectory, "*.zip")
                             .Select(ZipFile.Read))
            {
                Console.WriteLine(archive.Name);

                // reset developer since for a new file we don't know the developer
                Developer currentDeveloper = null;

                foreach (var evt in fileLoader.ReadAllEvents(archive))
                {
                    var ideSessionUUID = evt.IDESessionUUID;
                    if (IsDuplicate(eventsCollection, evt)) continue;
                    if (ideSessionUUID != null)
                    {
                        if (currentDeveloper == null)
                        {
                            currentDeveloper = FindOrCreateCurrentDeveloper(ideSessionUUID, developerCollection);
                        }
                        else
                        {
                            currentDeveloper.SessionIds.Add(ideSessionUUID);
                            developerCollection.Save(currentDeveloper);
                        }
                    }
                    eventsCollection.Insert(evt);
                }
            }
        }

        private static bool IsDuplicate(MongoCollection eventsCollection, IDEEvent evt)
        {
            return eventsCollection.Count(
                Query.And(
                    Query<IDEEvent>.EQ(e => e.IDESessionUUID, evt.IDESessionUUID),
                    Query<IDEEvent>.EQ(e => e.TriggeredAt, evt.TriggeredAt),
                    Query.EQ("_t", evt.GetType().Name))) > 0;
        }

        private static Developer FindOrCreateCurrentDeveloper(string ideSessionUUID,
            MongoCollection<Developer> developerCollection)
        {
            var query = Query<Developer>.EQ(dev => dev.SessionIds, ideSessionUUID);
            var candidates = developerCollection.Find(query).ToList();
            switch (candidates.Count)
            {
                case 0:
                    var newDeveloper = new Developer();
                    newDeveloper.SessionIds.Add(ideSessionUUID);
                    developerCollection.Insert(newDeveloper);
                    return newDeveloper;
                case 1:
                    return candidates.First();
                default:
                    throw new Exception("more than one developer with same session id");
            }
        }

        private static MongoDatabase GetDatabase()
        {
            var client = new MongoClient(DatabaseUrl);
            var server = client.GetServer();
            return server.GetDatabase(DatabaseName);
        }

        private static MongoCollection<T> GetCollection<T>(MongoDatabase database)
        {
            var collectionName = typeof (T).Name;
            if (!database.CollectionExists(collectionName))
            {
                database.CreateCollection(collectionName);
            }
            return database.GetCollection<T>(collectionName);
        }
    }
}
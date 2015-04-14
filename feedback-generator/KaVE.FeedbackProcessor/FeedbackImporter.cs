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
using System.IO;
using System.Linq;
using Ionic.Zip;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Exceptions;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace KaVE.FeedbackProcessor
{
    internal class FeedbackImporter
    {
        private static readonly ILogger Logger = new ConsoleLogger();

        private static void Main()
        {
            var database = new FeedbackDatabase(Configuration.DatabaseUrl, Configuration.DatabaseName);
            var eventsCollection = database.GetEventsCollection();
            var developerCollection = database.GetDeveloperCollection();

            Import(developerCollection, eventsCollection);
            LogDeveloperStatistics(developerCollection);
        }

        private static void LogDeveloperStatistics(MongoCollection<Developer> developerCollection)
        {
            var devs = developerCollection.FindAll();
            var sessIds = new HashSet<string>();
            var dupSessIds = new HashSet<string>();
            foreach (var sessionId in devs.SelectMany(developer => developer.SessionIds))
            {
                if (sessIds.Contains(sessionId))
                {
                    dupSessIds.Add(sessionId);
                }
                else
                {
                    sessIds.Add(sessionId);
                }
            }
            Logger.Info(string.Format("Found {0} developers.", devs.Count()));
            Logger.Info(string.Format("Found {0} sessions.", sessIds.Count));
            Logger.Info(string.Format("Found {0} duplicated sessions.", dupSessIds.Count));
        }

        private static void Import(MongoCollection<Developer> developerCollection,
            MongoCollection<IDEEvent> eventsCollection)
        {
            var fileLoader = new FileLoader();
            var totalNumberOfUniqueEvents = 0;
            var totalNumberOfDuplicatedEvents = 0;

            foreach (
                var archive in
                    Directory.GetFiles(Configuration.ImportDirectory, "*.zip")
                             .Select(ZipFile.Read))
            {
                Logger.Info(archive.Name);

                // reset developer since for a new file we don't know the developer
                Developer currentDeveloper = null;
                var numberOfUniqueEvents = 0;
                var numberOfDuplicatedEvents = 0;
                var numberNewOfSessions = 0;
                var foundEventOfDupId1 = false;
                var foundEventOfDupId2 = false;

                foreach (var evt in fileLoader.ReadAllEvents(archive))
                {
                    if (IsDuplicate(eventsCollection, evt))
                    {
                        numberOfDuplicatedEvents++;
                        continue;
                    }

                    var ideSessionUUID = evt.IDESessionUUID;
                    if (ideSessionUUID != null)
                    {
                        if ((ideSessionUUID.Equals("0a02cb7c-be92-43e2-8d8c-fcddbaa1d6cb") && !foundEventOfDupId1) ||
                            (ideSessionUUID.Equals("dc7e8b83-722d-4fd3-9c6d-86bd5ff30dff") && !foundEventOfDupId2))
                        {
                            foundEventOfDupId1 |= ideSessionUUID.Equals("0a02cb7c-be92-43e2-8d8c-fcddbaa1d6cb");
                            foundEventOfDupId2 |= ideSessionUUID.Equals("dc7e8b83-722d-4fd3-9c6d-86bd5ff30dff");
                            Logger.Error(string.Format("found event of session {0}", ideSessionUUID));
                        }

                        if (currentDeveloper == null)
                        {
                            currentDeveloper = FindOrCreateCurrentDeveloper(ideSessionUUID, developerCollection);
                            Logger.Info(string.Format(" developer {0} with {1} sessions.", currentDeveloper.Id, currentDeveloper.SessionIds.Count));
                        }
                        else
                        {
                            if (!currentDeveloper.SessionIds.Contains(ideSessionUUID)) numberNewOfSessions++;
                            currentDeveloper.SessionIds.Add(ideSessionUUID);
                            developerCollection.Save(currentDeveloper);
                        }
                    }

                    eventsCollection.Insert(evt);
                    numberOfUniqueEvents++;
                }

                Logger.Info(string.Format(" Added {0} new sessions.", numberNewOfSessions));
                Logger.Info(string.Format(" Inserted {0} events, filtered {1} duplicates.", numberOfUniqueEvents, numberOfDuplicatedEvents));
                totalNumberOfUniqueEvents += numberOfUniqueEvents;
                totalNumberOfDuplicatedEvents += numberOfDuplicatedEvents;
            }
            Logger.Info(string.Format("Inserted {0} events, filtered {1} duplicates.", totalNumberOfUniqueEvents, totalNumberOfDuplicatedEvents));
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
                    Logger.Error("More than one developer with the same session id encountered:");
                    Logger.Error(" - Session Id: " + ideSessionUUID);
                    Logger.Error(string.Format(" - {0} developers with same id:", candidates.Count));
                    candidates.ForEach(d => Logger.Error("   - Developer: " + d.Id));
                    throw new Exception("More than one developer with the same session id encountered");
            }
        }
    }
}
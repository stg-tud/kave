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
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Utils.Exceptions;
using KaVE.FeedbackProcessor.Database;
using KaVE.FeedbackProcessor.Model;
using KaVE.FeedbackProcessor.Properties;

namespace KaVE.FeedbackProcessor.Import
{
    internal class FeedbackImporter
    {
        private readonly IFeedbackDatabase _database;
        private static ILogger _logger;

        internal FeedbackImporter(IFeedbackDatabase database, ILogger logger)
        {
            _database = database;
            _logger = logger;
        }

        public void LogDeveloperStatistics()
        {
            var developerCollection = _database.GetDeveloperCollection();
            var devs = developerCollection.FindAll().ToList();
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
            _logger.Info(string.Format("Found {0} developers.", devs.Count()));
            _logger.Info(string.Format("Found {0} sessions.", sessIds.Count));
            _logger.Info(string.Format("Found {0} duplicated sessions.", dupSessIds.Count));
        }

        public void Import()
        {
            var eventsCollection = _database.GetOriginalEventsCollection();
            var developerCollection = _database.GetDeveloperCollection();

            eventsCollection.Clear();
            developerCollection.Clear();

            var fileLoader = new FeedbackArchiveReader();
            var totalNumberOfUniqueEvents = 0;
            var totalNumberOfDuplicatedEvents = 0;
            var nextArtificialTriggerTime = new DateTime();

            foreach (var archive in OpenFeedbackArchives().OrderBy(NumericalFilename))
            {
                _logger.Info(archive.Name);

                // reset developer since for a new file we don't know the developer
                Developer currentDeveloper = null;
                var numberOfUniqueEvents = 0;
                var numberOfDuplicatedEvents = 0;
                var numberNewOfSessions = 0;
                string ideSessionUUID = null;

                foreach (var evt in fileLoader.ReadAllEvents(archive))
                {
                    if (eventsCollection.Contains(evt))
                    {
                        numberOfDuplicatedEvents++;
                        continue;
                    }

                    var ideStateEvent = evt as IDEStateEvent;
                    if (ideStateEvent != null && ideStateEvent.IDESessionUUID == null)
                    {
                        ideStateEvent.IDESessionUUID = ideSessionUUID;
                    }

                    ideSessionUUID = evt.IDESessionUUID;
                    if (currentDeveloper == null)
                    {
                        currentDeveloper = FindOrCreateCurrentDeveloper(ideSessionUUID, developerCollection);
                        _logger.Info(
                            string.Format(
                                " developer {0} with {1} sessions.",
                                currentDeveloper.Id,
                                currentDeveloper.SessionIds.Count));
                    }
                    else if (ideSessionUUID != null)
                    {
                        if (!currentDeveloper.SessionIds.Contains(ideSessionUUID))
                        {
                            numberNewOfSessions++;
                        }
                        currentDeveloper.SessionIds.Add(ideSessionUUID);
                        developerCollection.Save(currentDeveloper);
                    }

                    if (ideSessionUUID == null)
                    {
                        evt.IDESessionUUID = currentDeveloper.Id.ToString();
                    }

                    if (evt.TriggeredAt == null)
                    {
                        evt.TriggeredAt = nextArtificialTriggerTime;
                        nextArtificialTriggerTime = nextArtificialTriggerTime.AddSeconds(1);
                    }

                    eventsCollection.Insert(evt);
                    numberOfUniqueEvents++;
                }

                _logger.Info(string.Format(" Added {0} new sessions.", numberNewOfSessions));
                _logger.Info(
                    string.Format(
                        " Inserted {0} events, filtered {1} duplicates.",
                        numberOfUniqueEvents,
                        numberOfDuplicatedEvents));
                totalNumberOfUniqueEvents += numberOfUniqueEvents;
                totalNumberOfDuplicatedEvents += numberOfDuplicatedEvents;
            }
            _logger.Info(
                string.Format(
                    "Inserted {0} events, filtered {1} duplicates.",
                    totalNumberOfUniqueEvents,
                    totalNumberOfDuplicatedEvents));
        }

        protected virtual IEnumerable<ZipFile> OpenFeedbackArchives()
        {
            return Directory.GetFiles(Configuration.ImportDirectory, "*.zip").Select(ZipFile.Read);
        }

        private static int NumericalFilename(ZipFile archive)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            // If there is an archive without a filename, I don't know what to do, so failing is the only option anyways.
            return int.Parse(Path.GetFileNameWithoutExtension(archive.Name));
        }

        private static Developer FindOrCreateCurrentDeveloper(string ideSessionUUID,
            IDeveloperCollection developerCollection)
        {
            if (ideSessionUUID == null)
            {
                return CreateAnonymousDeveloper(developerCollection);
            }
            var candidates = developerCollection.FindBySessionId(ideSessionUUID);
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
                    _logger.Error("More than one developer with the same session id encountered:");
                    _logger.Error(" - Session Id: " + ideSessionUUID);
                    _logger.Error(string.Format(" - {0} developers with same id:", candidates.Count));
                    foreach (var developer in candidates)
                    {
                        _logger.Error("   - Developer: " + developer.Id);
                    }
                    throw new Exception("More than one developer with the same session id encountered");
            }
        }

        private static Developer CreateAnonymousDeveloper(IDeveloperCollection developerCollection)
        {
            var anonymousDeveloper = new Developer();
            developerCollection.Insert(anonymousDeveloper);
            return anonymousDeveloper;
        }
    }
}
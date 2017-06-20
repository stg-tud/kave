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
using System.Text.RegularExpressions;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Exceptions;
using KaVE.FeedbackProcessor.Database;
using KaVE.FeedbackProcessor.Model;
using MongoDB.Driver.Builders;

namespace KaVE.FeedbackProcessor.Statistics
{
    internal class AnonymizationStatisticsLogger
    {
        private readonly IFeedbackDatabase _database;
        private readonly ILogger _logger;

        public AnonymizationStatisticsLogger(IFeedbackDatabase database, ILogger logger)
        {
            _database = database;
            _logger = logger;
        }

        private IIDEEventCollection EventsCollection
        {
            get { return _database.GetEventsCollection(); }
        }

        public void LogNumberOfEventsWithoutSessionId()
        {
            var developers =
                _database.GetDeveloperCollection().FindAll().Where(dev => dev.IsAnonymousSessionDeveloper).ToList();
            _logger.Info("Found {0} developer(s) of anonymous sessions.", developers.Count);

            foreach (var developer in developers)
            {
                var anonymousEvents =
                    EventsCollection.Find(Query<IDEEvent>.EQ(evt => evt.IDESessionUUID, developer.Id.ToString()));
                _logger.Info(" - Developer {0} with {1} event(s) for the session.", developer.Id, anonymousEvents.Count());
            }
        }

        public void LogNumberOfEventsWithoutTriggerTime()
        {
            var anonymousEvents =
                EventsCollection.Find(Query<IDEEvent>.LT(evt => evt.TriggeredAt, new DateTime(1000, 1, 1))).ToList();
            _logger.Info("Found {0} events without trigger time.", anonymousEvents.Count());
            var sessionIds = new HashSet<string>(anonymousEvents.Select(evt => evt.IDESessionUUID));
            _logger.Info(" - The events are from the sessions: {0}", sessionIds.ToStringReflection());
        }

        public void LogNumberOfEventsWithoutDuration()
        {
            var anonymousEvents =
                EventsCollection.Find(
                    Query.And(
                        Query.Or(
                            Query<IDEEvent>.NotExists(evt => evt.Duration),
                            Query<IDEEvent>.EQ(evt => evt.Duration, null)),
                        Query.NE("_t", "CommandEvent")));
            _logger.Info("Found {0} events without duration (approx. number of events anonymized for duration).", anonymousEvents.Count());
        }

        public void LogNumberOfSessionsWithAnonymizedNames()
        {
            var anonymizedWindowEvents =
                EventsCollection
                    .FindAs<WindowEvent>(Query.EQ("_t", "WindowEvent"))
                    .Where(IsAnonymized);
            var sessionsWithAnonymizedNames = new HashSet<string>(
                anonymizedWindowEvents.Select(we => we.IDESessionUUID));
            _logger.Info("Found {0} sessions with anonymized events.", sessionsWithAnonymizedNames.Count);
            var anonymizingDevelopers = GetDevelopersForSessionIds(sessionsWithAnonymizedNames);
            _logger.Info("These sessions belong to at most {0} developers.", anonymizingDevelopers.Count());
        }

        private IEnumerable<IList<Developer>> GetDevelopersForSessionIds(IEnumerable<string> sessionIds)
        {
            return sessionIds.Select(sessionId => _database.GetDeveloperCollection().FindBySessionId(sessionId));
        }

        private bool IsAnonymized(WindowEvent we)
        {
            var caption = we.Window.Caption;
            return (caption.Length == 24) &&
                   Regex.IsMatch(caption, @"^[a-zA-Z0-9-_]*={0,3}$", RegexOptions.None);
        }
    }
}
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
using System.Text.RegularExpressions;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Utils.Exceptions;
using KaVE.FeedbackProcessor.Database;
using KaVE.FeedbackProcessor.Model;
using MongoDB.Driver.Builders;

namespace KaVE.FeedbackProcessor.Statistics
{
    internal class AnonymizationStatisticsCalculator
    {
        private readonly IFeedbackDatabase _database;
        private readonly ILogger _logger;

        public AnonymizationStatisticsCalculator(IFeedbackDatabase database, ILogger logger)
        {
            _database = database;
            _logger = logger;
        }

        private IIDEEventCollection EventsCollection
        {
            get { return _database.GetOriginalEventsCollection(); }
        }

        public void LogNumberOfEventsWithoutSessionId()
        {
            var anonymousEvents =
                EventsCollection.Find(
                    Query.Or(
                        Query<IDEEvent>.NotExists(evt => evt.IDESessionUUID),
                        Query<IDEEvent>.EQ(evt => evt.IDESessionUUID, null)));
            _logger.Info(string.Format("Found {0} events without session id.", anonymousEvents.Count()));
        }

        public void LogNumberOfEventsWithoutTriggerTime()
        {
            var anonymousEvents =
                EventsCollection.Find(
                    Query.Or(
                        Query<IDEEvent>.NotExists(evt => evt.TriggeredAt),
                        Query<IDEEvent>.EQ(evt => evt.TriggeredAt, null)));
            _logger.Info(string.Format("Found {0} events without trigger time.", anonymousEvents.Count()));
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
            _logger.Info(
                string.Format(
                    "Found {0} events without duration (approx. number of events anonymized for duration).",
                    anonymousEvents.Count()));
        }

        public void LogNumberOfSessionsWithAnonymizedNames()
        {
            var anonymizedWindowEvents =
                EventsCollection
                    .FindAs<WindowEvent>(Query.EQ("_t", "WindowEvent"))
                    .Where(IsAnonymized);
            var sessionsWithAnonymizedNames = new HashSet<string>(
                anonymizedWindowEvents.Select(we => we.IDESessionUUID));
            _logger.Info(String.Format("Found {0} sessions with anonymized events.", sessionsWithAnonymizedNames.Count));
            var anonymizingDevelopers = GetDevelopersForSessionIds(sessionsWithAnonymizedNames);
            _logger.Info(String.Format("These sessions belong to at most {0} developers.", anonymizingDevelopers.Count()));
        }

        private IEnumerable<IList<Developer>> GetDevelopersForSessionIds(IEnumerable<string> sessionIds)
        {
            return sessionIds.Select(sessionId => _database.GetDeveloperCollection().FindBySessionId(sessionId));
        }

        private bool IsAnonymized(WindowEvent we)
        {
            var caption = we.Window.Caption;
            var isAnonymized = (caption.Length == 24) &&
                               Regex.IsMatch(caption, @"^[a-zA-Z0-9-_]*={0,3}$", RegexOptions.None);
            if (isAnonymized)
            {
                _logger.Info(caption + " ");
            }
            return isAnonymized;
        }
    }
}
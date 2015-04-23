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
using KaVE.Commons.Utils.Exceptions;
using KaVE.FeedbackProcessor.Database;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Statistics
{
    internal class DeveloperStatisticsCalculator
    {
        private readonly IFeedbackDatabase _feedbackDatabase;
        private readonly ILogger _logger;

        public DeveloperStatisticsCalculator(IFeedbackDatabase feedbackDatabase, ILogger logger)
        {
            _feedbackDatabase = feedbackDatabase;
            _logger = logger;
        }

        private IIDEEventCollection EventsCollection
        {
            get { return _feedbackDatabase.GetOriginalEventsCollection(); }
        }

        private IEnumerable<Developer> Developers
        {
            get { return _feedbackDatabase.GetDeveloperCollection().FindAll(); }
        }

        public ISet<DateTime> GetActiveDays(Developer developer)
        {
            return new HashSet<DateTime>(EventsCollection.GetEventStream(developer).Select(EventDate));
        }

        private static DateTime EventDate(IDEEvent evt)
        {
            var dateTime = evt.TriggeredAt;
            return dateTime.HasValue ? dateTime.Value.Date : new DateTime().Date;
        }

        public int GetLowerBoundToNumberOfParticipants()
        {
            var activeDaySets = Developers.Select(GetActiveDays).ToList();
            return activeDaySets.Select(dayset => IsOverlapingWithAllPrevious(activeDaySets, dayset)).Count(b => b);
        }

        private static bool IsOverlapingWithAllPrevious(IEnumerable<ISet<DateTime>> daySets,
            IEnumerable<DateTime> daySet)
        {
            var previousDateSets = daySets.TakeWhile(otherDaySet => !daySet.Equals(otherDaySet));
            return previousDateSets.All(set => set.Overlaps(daySet));
        }

        public int GetUpperBoundToNumberOfParticipants()
        {
            return Developers.Count();
        }

        public int GetNumberOfSessionsAssignedToMultipleDevelopers()
        {
            return
                Developers.SelectMany(dev => dev.SessionIds)
                          .GroupBy(sessionId => sessionId)
                          .Count(group => group.Count() > 1);
        }

        public int GetNumberOfSessions()
        {
            return new HashSet<string>(Developers.SelectMany(dev => dev.SessionIds)).Count;
        }

        public void LogDeveloperStatistic()
        {
            _logger.Info(string.Format("We have at most {0} developer(s).", GetUpperBoundToNumberOfParticipants()));
            _logger.Info(string.Format("We have at least {0} developer(s).", GetLowerBoundToNumberOfParticipants()));
            _logger.Info(string.Format("We have {0} session(s) in total.", GetNumberOfSessions()));
            var numberOfSessionsAssignedToMultipleDevelopers = GetNumberOfSessionsAssignedToMultipleDevelopers();
            if (numberOfSessionsAssignedToMultipleDevelopers > 0)
            {
                _logger.Error(
                    string.Format(
                        "We have {0} session(s) assigned to more than one developer!",
                        numberOfSessionsAssignedToMultipleDevelopers));
            }
        }
    }
}
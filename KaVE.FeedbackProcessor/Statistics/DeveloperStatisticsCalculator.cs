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
using KaVE.Commons.Model.Events;
using KaVE.FeedbackProcessor.Database;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Statistics
{
    internal class DeveloperStatisticsCalculator
    {
        private readonly IFeedbackDatabase _feedbackDatabase;

        public DeveloperStatisticsCalculator(IFeedbackDatabase feedbackDatabase)
        {
            _feedbackDatabase = feedbackDatabase;
        }

        private IIDEEventCollection EventsCollection
        {
            get { return _feedbackDatabase.GetEventsCollection(); }
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
            return evt.GetTriggeredAt().Date;
        }

        public int GetLowerBoundToNumberOfParticipants()
        {
            var activeDaySets = Developers.Where(dev => !dev.IsAnonymousSessionDeveloper).Select(GetActiveDays).ToList();

            while (TryToCombineDaySets(activeDaySets)) {}

            return activeDaySets.Count;
        }

        private static bool TryToCombineDaySets(IList<ISet<DateTime>> activeDaySets)
        {
            for (var devIdx = 0; devIdx < activeDaySets.Count; devIdx++)
            {
                var devActiveDays = activeDaySets[devIdx];
                for (var otherDevIdx = devIdx + 1; otherDevIdx < activeDaySets.Count; otherDevIdx++)
                {
                    var otherDevActiveDays = activeDaySets[otherDevIdx];
                    if (!devActiveDays.Overlaps(otherDevActiveDays))
                    {
                        activeDaySets.Remove(otherDevActiveDays);
                        activeDaySets.Remove(devActiveDays);
                        activeDaySets.Add(new HashSet<DateTime>(devActiveDays.Union(otherDevActiveDays)));
                        return true;
                    }
                }
            }
            return false;
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
    }
}
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
using KaVE.FeedbackProcessor.Database;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Statistics
{
    class DeveloperStatisticsCalculator
    {
        private readonly IFeedbackDatabase _feedbackDatabase;

        public DeveloperStatisticsCalculator(IFeedbackDatabase feedbackDatabase)
        {
            _feedbackDatabase = feedbackDatabase;
        }

        private IIDEEventCollection EventsCollection
        {
            get { return _feedbackDatabase.GetOriginalEventsCollection(); }
        }

        public IEnumerable<DateTime> GetActiveDays(Developer developer)
        {
            return new HashSet<DateTime>(EventsCollection.GetEventStream(developer).Select(EventDate));
        }

        private static DateTime EventDate(IDEEvent evt)
        {
            var dateTime = evt.TriggeredAt;
            return dateTime.HasValue ? dateTime.Value.Date : new DateTime().Date;
        }
    }
}

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
using KaVE.Commons.Utils.Assertion;
using KaVE.FeedbackProcessor.Database;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Cleanup
{
    internal class FeedbackCleaner
    {
        private readonly IFeedbackDatabase _database;
        private readonly ICollection<Type> _processors;

        public FeedbackCleaner(IFeedbackDatabase database)
        {
            _database = database;
            _processors = new List<Type>();
        }

        public void RegisterProcessor<TP>() where TP : IIDEEventProcessor, new()
        {
            _processors.Add(typeof (TP));
        }

        public void ProcessFeedback()
        {
            var developers = _database.GetDeveloperCollection().FindAll();
            foreach (var developer in developers)
            {
                ProcessEventStreamOf(developer);
            }
        }

        private void ProcessEventStreamOf(Developer developer)
        {
            var processors = CreateProcessors();
            foreach (var ideEvent in GetAllEventsOf(developer))
            {
                ProcessEvent(ideEvent, processors);
            }
        }

        private List<IIDEEventProcessor> CreateProcessors()
        {
            return _processors.Select(Activator.CreateInstance).Cast<IIDEEventProcessor>().ToList();
        }

        private IEnumerable<IDEEvent> GetAllEventsOf(Developer developer)
        {
            var events = _database.GetOriginalEventsCollection();
            return events.GetEventStream(developer);
        }

        private void ProcessEvent(IDEEvent originalEvent, IEnumerable<IIDEEventProcessor> processors)
        {
            var resultingEvent = originalEvent;
            foreach (var candidate in processors.Select(processor => processor.Process(originalEvent)))
            {
                if (IsDropSignal(candidate))
                {
                    if (IsReplacement(resultingEvent, originalEvent))
                    {
                        Asserts.Fail("cannot drop and replace an event");
                    }
                    resultingEvent = null;
                }
                else if (!candidate.Equals(originalEvent))
                {
                    if (IsDropSignal(resultingEvent))
                    {
                        Asserts.Fail("cannot drop and replace an event");
                    }
                    else if (IsReplacement(resultingEvent, originalEvent))
                    {
                        Asserts.Fail("cannot replace an event by two");
                    }
                    resultingEvent = candidate;
                }
            }

            if (resultingEvent != null)
            {
                _database.GetCleanEventsCollection().Insert(resultingEvent);
            }
        }

        private static bool IsReplacement(IDEEvent resultingEvent, IDEEvent originalEvent)
        {
            return !ReferenceEquals(resultingEvent, originalEvent);
        }

        private static bool IsDropSignal(IDEEvent candidate)
        {
            return candidate == null;
        }
    }
}
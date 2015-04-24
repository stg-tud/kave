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
 *    - Mattis Manfred Kämmerer
 *    - Markus Zimmermann
 */

using System;
using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Database;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Cleanup
{
    internal class FeedbackCleaner
    {
        private readonly IFeedbackDatabase _sourceDatabase;
        private readonly IFeedbackDatabase _targetDatabase;
        private readonly ICollection<Type> _processors;

        public FeedbackCleaner(IFeedbackDatabase sourceDatabase, IFeedbackDatabase targetDatabase)
        {
            _sourceDatabase = sourceDatabase;
            _targetDatabase = targetDatabase;
            _processors = new List<Type>();
        }

        public void RegisterProcessor<TP>() where TP : IIDEEventProcessor, new()
        {
            _processors.Add(typeof (TP));
        }

        public void ProcessFeedback()
        {
            _sourceDatabase.GetCleanEventsCollection().Clear();

            var developers = _sourceDatabase.GetDeveloperCollection().FindAll();
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
            var events = _sourceDatabase.GetOriginalEventsCollection();
            return events.GetEventStream(developer);
        }

        private void ProcessEvent(IDEEvent originalEvent, IEnumerable<IIDEEventProcessor> processors)
        {
            ISet<IDEEvent> resultingEventSet = new KaVEHashSet<IDEEvent>();
            var DropOriginalEvent = false;

            foreach (var intermediateEventSet in processors.Select(processor => processor.Process(originalEvent)))
            {
                if (IsDropOriginalEventSignal(intermediateEventSet,originalEvent))
                {
                    DropOriginalEvent = true;
                }
                resultingEventSet.UnionWith(intermediateEventSet);
            }

            if (DropOriginalEvent)
            {
                resultingEventSet.Remove(originalEvent);
            }
            
            InsertEventsToCleanEventCollection(resultingEventSet);
        }

        private void InsertEventsToCleanEventCollection(ISet<IDEEvent> resultingEventSet)
        {
            foreach (var ideEvent in resultingEventSet)
            {
                _sourceDatabase.GetCleanEventsCollection().Insert(ideEvent);
            }
        }

        private static bool IsDropOriginalEventSignal(ISet<IDEEvent> eventSet, IDEEvent originalEvent)
        {
            return !eventSet.Contains(originalEvent);
        }

    }
}
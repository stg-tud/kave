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

namespace KaVE.FeedbackProcessor
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

        private IEnumerable<IDEEvent> GetAllEventsOf(Developer developer)
        {
            var events = _database.GetEventsCollection();
            return events.GetEventStream(developer);
        }

        public void RegisterProcessor<TP>() where TP : IIDEEventProcessor, new()
        {
            _processors.Add(typeof(TP));
        }

        public void ProcessFeedback()
        {
            var developers = _database.GetDeveloperCollection().FindAll();
            foreach (var developer in developers)
            {
                var processors = _processors.Select(Activator.CreateInstance).Cast<IIDEEventProcessor>().ToList();
                foreach (var ideEvent in GetAllEventsOf(developer))
                {
                    foreach (var ideEventProcessor in processors)
                    {
                        ideEventProcessor.Process(ideEvent);
                    }
                }
            }
            
        }
    }

    public interface IIDEEventProcessor
    {
        IDEEvent Process(IDEEvent @event);
    }
}
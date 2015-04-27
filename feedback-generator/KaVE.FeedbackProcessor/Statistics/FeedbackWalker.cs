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
using MongoDB.Driver.Linq;

namespace KaVE.FeedbackProcessor.Statistics
{
    internal class FeedbackWalker
    {
        private readonly IFeedbackDatabase _sourceDatabase;
        private readonly ILogger _logger;
        private readonly ICollection<Type> _processors;

        public FeedbackWalker(IFeedbackDatabase sourceDatabase, ILogger logger)
        {
            _sourceDatabase = sourceDatabase;
            _logger = logger;
            _processors = new List<Type>();
        }

        public void RegisterProcessor<TP>() where TP : IIDEEventProcessor, new()
        {
            _processors.Add(typeof (TP));
        }

        public void ProcessFeedback()
        {
            var developers = _sourceDatabase.GetDeveloperCollection().FindAll();
            foreach (var developer in developers)
            {
                ProcessDeveloper(developer);
            }
        }

        private void ProcessDeveloper(Developer developer)
        {
            _logger.Info("Processing developer {0}", developer.Id);
            _logger.Info("- Creating processors...");
            var processors = CreateProcessors();
            _logger.Info("- Initializing Processors...");
            foreach (var processor in processors)
            {
                processor.Developer = developer;
            }
            _logger.Info("- Processing event stream...");
            foreach (var ideEvent in GetAllEventsOf(developer))
            {
                ProcessEvent(ideEvent, processors);
            }
            _logger.Info("- Finalizing...");
            foreach (var processor in processors)
            {
                processor.OnStreamEnds();
            }
        }

        private List<IIDEEventProcessor> CreateProcessors()
        {
            return _processors.Select(Activator.CreateInstance).Cast<IIDEEventProcessor>().ToList();
        }

        private IEnumerable<IDEEvent> GetAllEventsOf(Developer developer)
        {
            var events = _sourceDatabase.GetEventsCollection();
            return events.GetEventStream(developer);
        }

        private static void ProcessEvent(IDEEvent originalEvent, IEnumerable<IIDEEventProcessor> processors)
        {
            foreach (var processor in processors)
            {
                processor.Process(originalEvent);
            }
        }
    }
}
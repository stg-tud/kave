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

using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Exceptions;
using KaVE.FeedbackProcessor.Database;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor
{
    internal class FeedbackMapper
    {
        private readonly IFeedbackDatabase _sourceDatabase;
        private readonly IFeedbackDatabase _targetDatabase;
        private readonly ILogger _logger;
        private readonly ICollection<IEventMapper<IKaVESet<IDEEvent>>> _mappers;

        public FeedbackMapper(IFeedbackDatabase sourceDatabase, IFeedbackDatabase targetDatabase, ILogger logger)
        {
            _sourceDatabase = sourceDatabase;
            _targetDatabase = targetDatabase;
            _logger = logger;
            _mappers = new List<IEventMapper<IKaVESet<IDEEvent>>>();
        }

        public void RegisterMapper<TP>(TP mapper) where TP : IEventMapper<IKaVESet<IDEEvent>>
        {
            _mappers.Add(mapper);
        }

        public void MapFeedback()
        {
            _targetDatabase.GetDeveloperCollection().Clear();
            _targetDatabase.GetEventsCollection().Clear();

            var developers = _sourceDatabase.GetDeveloperCollection().FindAll();
            var index = 1;
            foreach (var developer in developers)
            {
                MapFeedbackOf(developer, index);
                index++;
            }
        }

        private void MapFeedbackOf(Developer developer, int index)
        {
            _logger.Info("Mapping feedback of developer {0} ({1})", index, developer.Id);
            _targetDatabase.GetDeveloperCollection().Insert(developer);

            foreach (var mapper in _mappers)
            {
                mapper.OnStreamStarts(developer);
            }
            foreach (var ideEvent in GetEventStream(developer))
            {
                MapEvent(ideEvent);
            }
            foreach (var mapper1 in _mappers)
            {
                InsertEventsToTargetEventCollection(mapper1.OnStreamEnds());
            }
            _logger.Info("- Finalizing...");
        }

        private IEnumerable<IDEEvent> GetEventStream(Developer developer)
        {
            var events = _sourceDatabase.GetEventsCollection();
            return events.GetEventStream(developer);
        }

        private void MapEvent(IDEEvent originalEvent)
        {
            var resultingEventSet = new KaVEHashSet<IDEEvent>();
            var dropOriginalEvent = false;

            foreach (var intermediateEventSet in _mappers.Select(mapper => mapper.Map(originalEvent)))
            {
                if (IsDropOriginalEventSignal(intermediateEventSet, originalEvent))
                {
                    dropOriginalEvent = true;
                }
                resultingEventSet.UnionWith(intermediateEventSet);
            }

            if (dropOriginalEvent)
            {
                resultingEventSet.Remove(originalEvent);
            }

            InsertEventsToTargetEventCollection(resultingEventSet);
        }

        private void InsertEventsToTargetEventCollection(IEnumerable<IDEEvent> resultingEventSet)
        {
            foreach (var ideEvent in resultingEventSet)
            {
                _targetDatabase.GetEventsCollection().Insert(ideEvent);
            }
        }

        private static bool IsDropOriginalEventSignal(ICollection<IDEEvent> eventSet, IDEEvent originalEvent)
        {
            return !eventSet.Contains(originalEvent);
        }
    }
}
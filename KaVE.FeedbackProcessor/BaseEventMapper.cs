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
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Model;
using KaVE.JetBrains.Annotations;

namespace KaVE.FeedbackProcessor
{
    internal abstract class BaseEventMapper : IEventMapper<IKaVESet<IDEEvent>>
    {
        protected delegate void Processor<in TEvent>([NotNull] TEvent currentEvent) where TEvent : IDEEvent;

        private readonly IList<KeyValuePair<Type, Processor<IDEEvent>>> _processors =
            new List<KeyValuePair<Type, Processor<IDEEvent>>>();

        private IDEEvent _currentEvent;
        private IKaVESet<IDEEvent> _answer;

        /// <summary>
        ///     Register a sub-processor for a type. Accepts only on registration per type, but allows registrations for
        ///     sub-/supertypes. If multiple sub-processors apply for one event, all are invoked in the order of registration.
        /// </summary>
        protected void RegisterFor<TEvent>(Processor<TEvent> processor) where TEvent : IDEEvent
        {
            Asserts.Not(_processors.Any(pair => pair.Key == typeof (TEvent)), "multiple processors for same event type");

            _processors.Add(
                new KeyValuePair<Type, Processor<IDEEvent>>(typeof (TEvent), evt => processor((TEvent) evt)));
        }

        public virtual void OnStreamStarts(Developer value) { }

        public IKaVESet<IDEEvent> Map(IDEEvent @event)
        {
            try
            {
                _currentEvent = @event;
                return GetAnswer(@event);
            }
            finally
            {
                _currentEvent = null;
            }
        }

        private IKaVESet<IDEEvent> GetAnswer(IDEEvent @event)
        {
            _answer = Sets.NewHashSet(@event);
            foreach (var processor in GetProcessorsFor(@event))
            {
                processor(@event);
            }
            return _answer;
        }

        private IEnumerable<Processor<IDEEvent>> GetProcessorsFor(IDEEvent @event)
        {
            return _processors.Where(e => e.Key.IsInstanceOfType(@event)).Select(e => e.Value);
        }

        protected void DropCurrentEvent()
        {
            _answer.Remove(_currentEvent);
        }

        protected void ReplaceCurrentEventWith(params IDEEvent[] replacementEvents)
        {
            DropCurrentEvent();
            Insert(replacementEvents);
        }

        protected void Insert(params IDEEvent[] additionalEvents)
        {
            foreach (var ideEvent in additionalEvents)
            {
                _answer.Add(ideEvent);
            }
        }

        public virtual IKaVESet<IDEEvent> OnStreamEnds()
        {
            return Sets.NewHashSet<IDEEvent>();
        }
    }
}
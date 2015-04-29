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
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Model;
using KaVE.JetBrains.Annotations;

namespace KaVE.FeedbackProcessor.Cleanup.Processors
{
    internal abstract class BaseProcessor : IIDEEventProcessor
    {
        private readonly IDictionary<Type, Processor<IDEEvent>> _processors =
            new Dictionary<Type, Processor<IDEEvent>>();

        private IDEEvent _currentEvent;
        private IKaVESet<IDEEvent> _answer;

        protected delegate void Processor<in TEvent>([NotNull] TEvent currentEvent) where TEvent : IDEEvent;

        public virtual Developer Developer
        {
            set { }
        }

        protected void RegisterFor<TEvent>(Processor<TEvent> processor) where TEvent : IDEEvent
        {
            Asserts.Not(IsProcessorForSubOrSuperTypeRegistered<TEvent>(), "multiple processors for same event type");

            _processors[typeof (TEvent)] = evt => processor((TEvent) evt);
        }

        private bool IsProcessorForSubOrSuperTypeRegistered<TEvent>() where TEvent : IDEEvent
        {
            var et = typeof (TEvent);
            return _processors.Any(e => e.Key.IsAssignableFrom(et) || et.IsAssignableFrom(e.Key));
        }

        public IKaVESet<IDEEvent> Process(IDEEvent @event)
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
            Processor<IDEEvent> processor;
            if (TryGetProcessor(@event, out processor))
            {
                processor(@event);
            }
            return _answer;
        }

        private bool TryGetProcessor(IDEEvent @event, out Processor<IDEEvent> processor)
        {
            processor = _processors.FirstOrDefault(e => e.Key.IsInstanceOfType(@event)).Value;
            return processor != null;
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
    }
}
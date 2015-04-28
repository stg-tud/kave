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
using KaVE.FeedbackProcessor.Model;
using KaVE.JetBrains.Annotations;

namespace KaVE.FeedbackProcessor.Statistics
{
    abstract class BaseEventProcessor : IEventProcessor
    {
        protected delegate void Processor<in TEvent>([NotNull] TEvent @event) where TEvent : IDEEvent;

        private readonly IDictionary<Type, Processor<IDEEvent>> _processors =
            new Dictionary<Type, Processor<IDEEvent>>();

        protected void RegisterFor<TEvent>(Processor<TEvent> processor) where TEvent : IDEEvent
        {
            Asserts.Not(_processors.ContainsKey(typeof(TEvent)), "multiple processors for same event type");

            _processors[typeof(TEvent)] = evt => processor((TEvent)evt);
        }

        public virtual void OnStreamStarts(Developer value)
        { 
        }

        public void OnEvent(IDEEvent @event)
        {
            foreach (var processor in GetProcessorsFor(@event))
            {
                processor(@event);
            }
        }

        private IEnumerable<Processor<IDEEvent>> GetProcessorsFor(IDEEvent @event)
        {
            return _processors.Where(e => e.Key.IsInstanceOfType(@event)).Select(e => e.Value);
        }

        public void OnStreamEnds()
        {
        }
    }
}

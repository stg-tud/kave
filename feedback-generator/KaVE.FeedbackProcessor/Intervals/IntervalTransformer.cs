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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Exceptions;
using KaVE.FeedbackProcessor.Intervals.Model;
using KaVE.FeedbackProcessor.Intervals.Transformers;

namespace KaVE.FeedbackProcessor.Intervals
{
    public class IntervalTransformer
    {
        private readonly List<IEventToIntervalTransformer<Interval>> _transformers;
        private Queue<Interval> _queue; 

        public IntervalTransformer()
        {
            _transformers = new List<IEventToIntervalTransformer<Interval>>();
            _queue = new Queue<Interval>();

            RegisterTransformer(new VisualStudioOpenedTransformer());
            RegisterTransformer(new UserActiveTransformer());
        }

        private void RegisterTransformer<T>(IEventToIntervalTransformer<T> transformer)
            where T : Interval
        {
            transformer.IntervalCompleted += Transformer_IntervalCompleted;
            _transformers.Add(transformer);
        }

        private void Transformer_IntervalCompleted(Interval interval)
        {
            _queue.Enqueue(interval);
        }

        public IEnumerable<Interval> Transform(IEnumerable<IDEEvent> events)
        {
            // Events are usually not saved in order (neither by trigger nor by termination time)
            // so we have to sort them before processing. Maybe there is a better solution ...
            events = events.OrderBy(e => e.TriggeredAt);

            // TODO: provide developer parameter?
            _transformers.ForEach(t => t.OnStreamStarts(null));

            foreach (var e in events)
            {
                _transformers.ForEach(t => t.OnEvent(e));

                while (_queue.Count > 0)
                {
                    yield return _queue.Dequeue();
                }
            }

            _transformers.ForEach(t => t.OnStreamEnds());

            while (_queue.Count > 0)
            {
                yield return _queue.Dequeue();
            }
        }
    }
}

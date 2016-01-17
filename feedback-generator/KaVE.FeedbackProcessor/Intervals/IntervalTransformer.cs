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
using System.IO;
using KaVE.Commons.Model.Events;
using KaVE.FeedbackProcessor.Intervals.Model;
using KaVE.FeedbackProcessor.Intervals.Transformers;

namespace KaVE.FeedbackProcessor.Intervals
{
    public class IntervalTransformer
    {
        public IEnumerable<Interval> Transform(IEnumerable<IDEEvent> events)
        {
            var transformer =
                new ZeroLengthIntervalFilterTransformer(
                    new AggregateTransformer(
                        new VisualStudioOpenedTransformer(),
                        new SessionIdSortingTransformer<Interval>(() => 
                            new AggregateTransformer(
                                new UserActiveTransformer(),
                                new PerspectiveTransformer(),
                                new FileOpenTransformer()
                            )
                        )
                    )
                );
            return TransformWithCustomTransformer(events, transformer);

            //foreach (var e in TransformWithCustomTransformer(events, new VisualStudioOpenedTransformer()))
            //{
            //    yield return e;
            //}

            //foreach (var e in TransformWithCustomTransformer(events, new UserActiveTransformer()))
            //{
            //    yield return e;
            //}

            //foreach (var e in TransformWithCustomTransformer(events, new PerspectiveTransformer()))
            //{
            //    yield return e;
            //}

            //foreach (var e in TransformWithCustomTransformer(events, new FileOpenTransformer()))
            //{
            //    yield return e;
            //}

            //foreach (var e in TransformWithCustomTransformer(events, new FileInteractionTransformer()))
            //{
            //    yield return e;
            //}
        }

        public static IEnumerable<Interval> TransformWithCustomTransformer(IEnumerable<IDEEvent> events,
            IEventToIntervalTransformer<Interval> transformer)
        {
            Console.WriteLine(@"Transforming event stream with {0} ...", transformer.GetType().Name);

            var currentEventTime = DateTime.MinValue;
            int i = 0;
            foreach (var e in events)
            {
                Console.Write("\rProcessed {0} events ...", ++i);

                if (TransformerUtils.EventHasNoTimeData(e))
                {
                    continue;
                }

                if (e.TriggeredAt.GetValueOrDefault() < currentEventTime)
                {
                    throw new InvalidDataException("Event stream must be ordered by the 'TriggeredAt' property.");
                }

                currentEventTime = e.TriggeredAt.GetValueOrDefault();

                transformer.ProcessEvent(e);
            }

            Console.WriteLine();

            return transformer.SignalEndOfEventStream();
        }
    }
}
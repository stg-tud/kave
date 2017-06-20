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
using Ionic.Zip;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Exceptions;
using KaVE.FeedbackProcessor.Import;
using KaVE.FeedbackProcessor.WatchdogExports.Model;
using KaVE.FeedbackProcessor.WatchdogExports.Transformers;

namespace KaVE.FeedbackProcessor.WatchdogExports
{
    public class IntervalTransformer
    {
        private readonly ILogger _logger;
        private readonly IEventFixer _fixer;
        private readonly int _firstVersionToInclude;

        public IntervalTransformer(ILogger logger, IEventFixer fixer, int firstVersionToInclude)
        {
            Asserts.That(firstVersionToInclude >= 0);
            _logger = logger;
            _fixer = fixer;
            _firstVersionToInclude = firstVersionToInclude;
        }

        private static ZeroLengthIntervalFilterTransformer CreateDefaultTransformer()
        {
            var context = new TransformerContext();
            var transformer =
                new ZeroLengthIntervalFilterTransformer(
                    new AggregateTransformer(
                        context,
                        new VisualStudioOpenedTransformer(context),
                        new TestRunIntervalTransformer(context),
                        new SessionIdSortingTransformer<Interval>(
                            () =>
                                new AggregateTransformer(
                                    new VisualStudioActiveTransformer(context),
                                    new UserActiveTransformer(context),
                                    new PerspectiveTransformer(context),
                                    //new FileOpenTransformer(context),
                                    new FileInteractionTransformer(context)
                                    )
                            )
                        )
                    );
            return transformer;
        }

        public IEnumerable<Interval> TransformFile(string filename)
        {
            return TransformFileWithCustomTransformer(filename, CreateDefaultTransformer());
        }

        public IEnumerable<Interval> TransformFileWithCustomTransformer(string filename,
            IEventToIntervalTransformer<Interval> transformer)
        {
            _logger.Info("Loading events from {0} ...", filename);
            var events = GetAllEventsFromFile(filename);
            var intervals = TransformWithCustomTransformer(events, transformer);
            return SetUserId(intervals, filename);
        }

        public IEnumerable<Interval> TransformWithCustomTransformer(IEnumerable<IDEEvent> events,
            IEventToIntervalTransformer<Interval> transformer)
        {
            events = _fixer.FixAndFilter(events);

            _logger.Info(@"Transforming event stream with {0} ...", transformer.GetType().Name);

            var currentEventTime = DateTime.MinValue;
            var i = 0;
            foreach (var e in events)
            {
                i++;
                if (i%500 == 0)
                {
                    Console.Write('.');
                }
                if (i%50000 == 0)
                {
                    Console.WriteLine();
                }

                if (TransformerUtils.EventHasNoTimeData(e))
                {
                    continue;
                }

                var version = VersionUtil.Parse(e.KaVEVersion);
                if (version.KaVEVersionNumber < _firstVersionToInclude)
                {
                    continue;
                }

                Asserts.IsLessOrEqual(currentEventTime, e.TriggeredAt.GetValueOrDefault());

                currentEventTime = e.TriggeredAt.GetValueOrDefault();

                transformer.ProcessEvent(e);
            }
            Console.WriteLine();
            _logger.Info("--> Done processing {0} events...", i);

            return transformer.SignalEndOfEventStream();
        }

        private static IEnumerable<Interval> SetUserId(IEnumerable<Interval> intervals, string userId)
        {
            return intervals.Select(
                i =>
                {
                    i.UserId = userId;
                    return i;
                });
        }

        private static IEnumerable<IDEEvent> GetAllEventsFromFile(string file)
        {
            var zip = ZipFile.Read(file);
            var fileLoader = new FeedbackArchiveReader();
            return fileLoader.ReadAllEvents(zip);
        }
    }
}
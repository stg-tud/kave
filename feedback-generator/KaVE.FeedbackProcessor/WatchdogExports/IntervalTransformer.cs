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
using System.Linq;
using Ionic.Zip;
using KaVE.Commons.Model.Events;
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

        public IntervalTransformer(ILogger logger, IEventFixer fixer)
        {
            _logger = logger;
            _fixer = fixer;
        }

        public IEnumerable<Interval> Transform(IEnumerable<IDEEvent> events)
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

            return TransformWithCustomTransformer(events, transformer);
        }

        public IEnumerable<Interval> TransformFile(string filename)
        {
            Console.WriteLine("Transforming {0}...", filename);
            return
                Transform(GetAllEventsFromFile(filename)).Select(SetUserId(Path.GetFileNameWithoutExtension(filename)));
        }

        public IEnumerable<Interval> TransformFolder(string path)
        {
            var zips = Directory.GetFiles(path, "*.zip", SearchOption.AllDirectories);
            return zips.SelectMany(TransformFile);
        }

        public IEnumerable<Interval> TransformWithCustomTransformer(IEnumerable<IDEEvent> events,
            IEventToIntervalTransformer<Interval> transformer)
        {
            events = _fixer.FixAndFilter(events);

            _logger.Info(@"Transforming event stream with {0} ...", transformer.GetType().Name);

            var currentEventTime = DateTime.MinValue;
            int i = 0;
            foreach (var e in events)
            {
                if ((i++)%5000 == 0)
                {
                    _logger.Info("Processed {0} events ...", i - 1);
                }

                if (TransformerUtils.EventHasNoTimeData(e))
                {
                    continue;
                }

                Asserts.IsLessOrEqual(currentEventTime, e.TriggeredAt.GetValueOrDefault());

                currentEventTime = e.TriggeredAt.GetValueOrDefault();

                transformer.ProcessEvent(e);
            }

            return transformer.SignalEndOfEventStream();
        }

        public IEnumerable<Interval> TransformFileWithCustomTransformer(string filename,
            IEventToIntervalTransformer<Interval> transformer)
        {
            _logger.Info("Loading events from {0} ...", filename);
            return
                TransformWithCustomTransformer(GetAllEventsFromFile(filename), transformer)
                    .Select(SetUserId(Path.GetFileNameWithoutExtension(filename)));
        }

        private Func<Interval, Interval> SetUserId(string filename)
        {
            return i =>
            {
                i.UserId = filename;
                return i;
            };
        }

        private IEnumerable<IDEEvent> GetAllEventsFromFile(string file)
        {
            var zip = ZipFile.Read(file);
            var fileLoader = new FeedbackArchiveReader();
            return fileLoader.ReadAllEvents(zip);
        }
    }
}
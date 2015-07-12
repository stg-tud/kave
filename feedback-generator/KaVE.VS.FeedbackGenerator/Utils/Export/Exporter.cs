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
using JetBrains;
using JetBrains.Application;
using JetBrains.Util;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Json;
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Generators;
using KaVE.VS.FeedbackGenerator.SessionManager.Anonymize;

namespace KaVE.VS.FeedbackGenerator.Utils.Export
{
    public interface IExporter
    {
        event Action<string> StatusChanged;
        void Export(IList<IDEEvent> events, IPublisher publisher);
    }

    [ShellComponent]
    public class Exporter : IExporter
    {
        private readonly IDataExportAnonymizer _anonymizer;
        private readonly IUserProfileEventGenerator _exportEventGenerator;

        public Exporter(IDataExportAnonymizer anonymizer, IUserProfileEventGenerator exportEventGenerator)
        {
            _exportEventGenerator = exportEventGenerator;
            _anonymizer = anonymizer;
        }

        public event Action<string> StatusChanged = s => { };

        public void Export(IList<IDEEvent> events, IPublisher publisher)
        {
            var isEmptyEventList = events.IsEmpty();
            var shouldCreateUserProfileEvent = _exportEventGenerator.ShouldCreateEvent();
            if (isEmptyEventList && !shouldCreateUserProfileEvent)
            {
                return;
            }

            if (shouldCreateUserProfileEvent)
            {
                events.Add(_exportEventGenerator.CreateEvent());
            }

            using (var stream = new MemoryStream())
            {
                var anonymousEvents = events.Select(_anonymizer.Anonymize).ToList();

                WriteEventsToZipStream(anonymousEvents, events.Count, stream);
                StatusChanged(Properties.UploadWizard.PublishingEvents);
                publisher.Publish(stream);
            }
        }

        private void WriteEventsToZipStream(IEnumerable<IDEEvent> events, int numberOfEvents, Stream stream)
        {
            using (var zipFile = new ZipFile())
            {
                zipFile.UseZip64WhenSaving = Zip64Option.AsNecessary;
                var i = 0;
                ReportExportProgress(i, numberOfEvents);
                foreach (var e in events)
                {
                    var fileName = (i++) + "-" + e.GetType().Name + ".json";
                    var json = e.ToFormattedJson();
                    zipFile.AddEntry(fileName, json);
                    ReportExportProgress(i, numberOfEvents);
                }
                StatusChanged(Properties.UploadWizard.CompressingEvents);
                zipFile.Save(stream);
            }
        }

        private void ReportExportProgress(int eventsExported, int totalNumberOfEvents)
        {
            var progress = eventsExported*100/totalNumberOfEvents;
            StatusChanged(Properties.UploadWizard.WritingEvents.FormatEx(progress));
        }
    }
}
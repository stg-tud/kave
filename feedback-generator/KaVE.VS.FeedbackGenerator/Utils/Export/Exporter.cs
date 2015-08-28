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
using KaVE.VS.FeedbackGenerator.Utils.Logging;

namespace KaVE.VS.FeedbackGenerator.Utils.Export
{
    public interface IExporter
    {
        event Action ExportStarted;
        event Action<string> StatusChanged;
        event Action ExportEnded;

        /// <summary>
        /// Exports all events before the given export time. Returns the number of exported events.
        /// </summary>
        int Export(DateTime exportTime, IPublisher publisher);
    }

    [ShellComponent]
    public class Exporter : IExporter
    {
        private readonly ILogManager _logManager;
        private readonly IDataExportAnonymizer _anonymizer;
        private readonly IUserProfileEventGenerator _profileEventGenerator;

        public Exporter(ILogManager logManager,
            IDataExportAnonymizer anonymizer,
            IUserProfileEventGenerator profileEventGenerator)
        {
            _logManager = logManager;
            _anonymizer = anonymizer;
            _profileEventGenerator = profileEventGenerator;
        }

        public event Action ExportStarted = () => { };
        public event Action<string> StatusChanged = s => { };
        public event Action ExportEnded = () => { };

        public int Export(DateTime exportTime, IPublisher publisher)
        {
            ExportStarted();
            try
            {
                var events = LoadEventsToExport(exportTime);
                MaybeAppendUserProfile(events);
                if (SomethingToExport(events))
                {
                    DoExport(events, publisher);
                }
                return events.Count;
            }
            finally
            {
                ExportEnded();
            }
        }

        private ICollection<IDEEvent> LoadEventsToExport(DateTime exportTime)
        {
            StatusChanged(Properties.UploadWizard.FetchingEvents);
            var events = new List<IDEEvent>();
            foreach (var log in _logManager.Logs)
            {
                events.AddRange(log.ReadAll().Where(e => e.TriggeredAt <= exportTime));
            }
            return events;
        }

        private void MaybeAppendUserProfile(ICollection<IDEEvent> events)
        {
            if (_profileEventGenerator.ShouldCreateEvent())
            {
                events.Add(_profileEventGenerator.CreateEvent());
            }
        }

        private static bool SomethingToExport(ICollection<IDEEvent> events)
        {
            return !events.IsEmpty();
        }

        private void DoExport(ICollection<IDEEvent> events, IPublisher publisher)
        {
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
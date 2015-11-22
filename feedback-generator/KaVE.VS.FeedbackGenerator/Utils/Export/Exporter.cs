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
using JetBrains.Application;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Generators;
using KaVE.VS.FeedbackGenerator.SessionManager.Anonymize;
using KaVE.VS.FeedbackGenerator.Utils.Logging;

namespace KaVE.VS.FeedbackGenerator.Utils.Export
{
    public interface IExporter
    {
        event Action ExportStarted;
        event Action<int> ExportProgressChanged;
        event Action ExportEnded;

        /// <summary>
        ///     Exports all events before the given export time. Returns the number of exported events.
        /// </summary>
        int Export(DateTime to, IPublisher publisher);
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

        public event Action ExportStarted;
        public event Action<int> ExportProgressChanged;
        public event Action ExportEnded;

        public int Export(DateTime to, IPublisher publisher)
        {
            OnExportStarted();
            try
            {
                var events = LoadEventsToExport(to);
                var numEvents = EstimateNumberEventsToExport();
                var upe = CreateUserProfileEvent();
                var progressReporter = CreateProgressReporter(numEvents);

                var lastEventNumber = 0;
                Action progressCallback = () =>
                {
                    lastEventNumber++;
                    progressReporter(lastEventNumber);
                };

                if (numEvents > 0)
                {
                    publisher.Publish(upe, events, progressCallback);
                }

                return lastEventNumber;
            }
            finally
            {
                OnExportEnded();
            }
        }

        private IEnumerable<IDEEvent> LoadEventsToExport(DateTime exportTime)
        {
            return
                _logManager.Logs.SelectMany(log => log.ReadAll())
                           .Where(e => e.TriggeredAt <= exportTime)
                           .Select(_anonymizer.Anonymize);
        }

        private int EstimateNumberEventsToExport()
        {
            return _logManager.Logs.Sum(l => l.ApproximateNumberOfEvents);
        }

        private UserProfileEvent CreateUserProfileEvent()
        {
            return _profileEventGenerator.ShouldCreateEvent() ? _profileEventGenerator.CreateEvent() : null;
        }

        private Action<int> CreateProgressReporter(int totalEvents)
        {
            var currentProgress = 0;
            return eventNumber =>
            {
                var oldProgress = currentProgress;
                currentProgress = Math.Min(eventNumber*100/totalEvents, 100);
                if (oldProgress != currentProgress && ExportProgressChanged != null)
                {
                    OnExportProgressChanged(currentProgress);
                }
            };
        }

        private void OnExportStarted()
        {
            var handlers = ExportStarted;
            if (handlers != null)
            {
                handlers();
            }
        }

        private void OnExportProgressChanged(int currentProgress)
        {
            var handlers = ExportProgressChanged;
            if (handlers != null)
            {
                handlers(currentProgress);
            }
        }

        private void OnExportEnded()
        {
            var handlers = ExportEnded;
            if (handlers != null)
            {
                handlers();
            }
        }
    }
}
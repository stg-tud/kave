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
using JetBrains.UI.Extensions.Commands;
using KaVE.Model.Events;
using KaVE.VsFeedbackGenerator.SessionManager;
using KaVE.VsFeedbackGenerator.Utils.Json;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public class ExportCommandFactory
    {
        private readonly IFeedbackViewModelDialog _feedbackViewModelDialog;
        private readonly IIoUtils _ioUtils = Registry.GetComponent<IIoUtils>();

        public ExportCommandFactory(IFeedbackViewModelDialog feedbackViewModelDialog)
        {
            _feedbackViewModelDialog = feedbackViewModelDialog;
        }

        public DelegateCommand Create(IPublisher publisher)
        {
            Action<object> action = o =>
            {
                try
                {
                    var events = _feedbackViewModelDialog.ExtractEventsForExport();
                    Export(events, publisher);
                    _feedbackViewModelDialog.ShowExportSucceededMessage(events.Count);
                }
                catch (Exception e)
                {
                    _feedbackViewModelDialog.ShowExportFailedMessage(e.Message);
                }
            };
            return new DelegateCommand(action, o => _feedbackViewModelDialog.AreAnyEventsPresent);
        }


        private void Export(IEnumerable<IDEEvent> events, IPublisher publisher)
        {
            var tempFileName = _ioUtils.GetTempFileName();
            using (var stream = _ioUtils.OpenFile(tempFileName, FileMode.Open, FileAccess.Write))
            {
                using (var logWriter = new JsonLogWriter<IDEEvent>(stream))
                {
                    logWriter.WriteAll(events);
                }
            }
            publisher.Publish(tempFileName);
        }
    }
}
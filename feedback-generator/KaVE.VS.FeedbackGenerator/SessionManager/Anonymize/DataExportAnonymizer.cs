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
using JetBrains.Application;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Utils.Json;
using KaVE.VS.FeedbackGenerator.SessionManager.Anonymize.CompletionEvents;
using KaVE.VS.FeedbackGenerator.SessionManager.Presentation;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.Utils;

namespace KaVE.VS.FeedbackGenerator.SessionManager.Anonymize
{
    public interface IDataExportAnonymizer
    {
        IDEEvent Anonymize(IDEEvent ideEvent);
    }

    [ShellComponent]
    public class DataExportAnonymizer : IDataExportAnonymizer
    {
        private static readonly IDictionary<Type, IDEEventAnonymizer> Anonymizer = new Dictionary
            <Type, IDEEventAnonymizer>
        {
            {typeof (BuildEvent), new BuildEventAnonymizer()},
            {typeof (SolutionEvent), new SolutionEventAnonymizer()},
            {typeof (DocumentEvent), new DocumentEventAnonymizer()},
            {typeof (WindowEvent), new WindowEventAnonymizer()},
            {typeof (ErrorEvent), new ErrorEventAnonymizer()},
            {typeof (InfoEvent), new InfoEventAnonymizer()},
            {typeof (IDEStateEvent), new IDEStateEventAnonymizer()},
            {typeof (CompletionEvent), new CompletionEventAnonymizer()}
        };

        private readonly ISettingsStore _settingsStore;

        public DataExportAnonymizer(ISettingsStore settingsStore)
        {
            _settingsStore = settingsStore;
        }

        public IDEEvent Anonymize(IDEEvent ideEvent)
        {
            var clone = ideEvent.ToCompactJson().ParseJsonTo<IDEEvent>();
            var settings = _settingsStore.GetSettings<ExportSettings>();
            var anonymizer = GetAnonymizerFor(ideEvent.GetType());
            if (settings.RemoveSessionIDs)
            {
                anonymizer.AnonymizeSessionUUID(clone);
            }
            if (settings.RemoveStartTimes)
            {
                anonymizer.AnonymizeStartTimes(clone);
            }
            if (settings.RemoveDurations)
            {
                anonymizer.AnonymizeDurations(clone);
            }
            if (settings.RemoveCodeNames)
            {
                anonymizer.AnonymizeCodeNames(clone);
            }
            return clone;
        }

        private static IDEEventAnonymizer GetAnonymizerFor(Type eventType)
        {
            return Anonymizer.ContainsKey(eventType) ? Anonymizer[eventType] : new IDEEventAnonymizer();
        }
    }
}
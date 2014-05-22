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
using JetBrains.Application;
using KaVE.Model.Events;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Events.VisualStudio;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Json;

namespace KaVE.VsFeedbackGenerator.SessionManager.Anonymize
{
    [ShellComponent]
    public class DataExportAnonymizer
    {
        private static readonly IDictionary<Type, object> Anonymizer = new Dictionary<Type, object>
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

        private static IDEEventAnonymizer<TEvent> GetAnonymizerFor<TEvent>() where TEvent : IDEEvent
        {
            if (Anonymizer.ContainsKey(typeof (TEvent)))
            {
                return (IDEEventAnonymizer<TEvent>) Anonymizer[typeof (TEvent)];
            }
            return new IDEEventAnonymizer<TEvent>();
        }

        public TEvent Anonymize<TEvent>(TEvent ideEvent) where TEvent : IDEEvent
        {
            var clone = ideEvent.ToCompactJson().ParseJsonTo<TEvent>();
            var settings = _settingsStore.GetSettings<ExportSettings>();
            var anonymizer = GetAnonymizerFor<TEvent>();
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
    }
}
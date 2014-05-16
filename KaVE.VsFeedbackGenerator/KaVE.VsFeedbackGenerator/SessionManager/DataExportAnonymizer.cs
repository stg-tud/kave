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
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Application;
using JetBrains.Util;
using KaVE.Model.Events;
using KaVE.Model.Events.VisualStudio;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Json;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    [ShellComponent]
    public class DataExportAnonymizer
    {
        private static readonly IDictionary<Type, object> Anonymizer = new Dictionary<Type, object>
        {
            {typeof (BuildEvent), new BuildEventAnonymizer()}
        };

        private readonly ISettingsStore _settingsStore;

        public DataExportAnonymizer(ISettingsStore settingsStore)
        {
            _settingsStore = settingsStore;
        }

        private IDEEventAnonymizer<TEvent> GetAnonymizerFor<TEvent>() where TEvent : IDEEvent
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

    internal class IDEEventAnonymizer<TEvent> where TEvent : IDEEvent
    {
        protected string CreateHash(string value)
        {
            var tmpSource = value.AsBytes();
            var hash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
            return Convert.ToBase64String(hash);
        }

        public virtual void AnonymizeSessionUUID(TEvent ideEvent)
        {
            ideEvent.IDESessionUUID = null;
        }

        public virtual void AnonymizeStartTimes(TEvent ideEvent)
        {
            ideEvent.TriggeredAt = null;
        }

        public virtual void AnonymizeDurations(TEvent ideEvent)
        {
            ideEvent.Duration = null;
        }

        public virtual void AnonymizeCodeNames(TEvent ideEvent) {}
    }

    internal class BuildEventAnonymizer : IDEEventAnonymizer<BuildEvent>
    {
        public override void AnonymizeStartTimes(BuildEvent ideEvent)
        {
            ForEachTargetDo(ideEvent, target => target.StartedAt = null);
            base.AnonymizeStartTimes(ideEvent);
        }

        public override void AnonymizeDurations(BuildEvent ideEvent)
        {
            ForEachTargetDo(ideEvent, target => target.Duration = null);
            base.AnonymizeDurations(ideEvent);
        }

        public override void AnonymizeCodeNames(BuildEvent ideEvent)
        {
            ForEachTargetDo(ideEvent, target => target.Project = CreateHash(target.Project));
            base.AnonymizeCodeNames(ideEvent);
        }

        private static void ForEachTargetDo(BuildEvent buildEvent, Action<BuildTarget> modify)
        {
            buildEvent.Targets.ForEach(modify);
        }
    }
}
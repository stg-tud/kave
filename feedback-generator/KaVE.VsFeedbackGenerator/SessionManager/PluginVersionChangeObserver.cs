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
 *    - Dennis Albrecht
 */

using JetBrains.Application;
using JetBrains.Application.Extensions;
using KaVE.Model.Events.VisualStudio;
using KaVE.VsFeedbackGenerator.Generators;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.VsIntegration;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    [ShellComponent]
    public class PluginVersionChangeObserver : EventGeneratorBase
    {
        public PluginVersionChangeObserver(ISettingsStore store,
            IRSEnv rsEnv,
            IIDESession session,
            IMessageBus messageBus,
            IDateUtils dateUtils) : base(session, messageBus, dateUtils)
        {
            var storedPluginVersion = store.GetSettings<FeedbackSettings>().PluginVersion;
            var currentPluginVersion = rsEnv.KaVEExtension.Version.ToString();

            if (storedPluginVersion != currentPluginVersion)
            {
                if (storedPluginVersion == string.Empty)
                {
                    var updateEvent = Create<InstallEvent>();
                    updateEvent.PluginVersion = currentPluginVersion;
                    FireNow(updateEvent);
                }
                else
                {
                    var updateEvent = Create<UpdateEvent>();
                    updateEvent.OldPluginVersion = storedPluginVersion;
                    updateEvent.NewPluginVersion = currentPluginVersion;
                    FireNow(updateEvent);
                }
                store.UpdateSettings<FeedbackSettings>(s => s.PluginVersion = currentPluginVersion);
            }
        }
    }

    public interface IRSEnv
    {
        IExtension KaVEExtension { get; }
    }
}
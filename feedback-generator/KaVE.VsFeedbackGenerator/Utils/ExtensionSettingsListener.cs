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
 *    - Uli Fahrer
 */

using JetBrains.Application;
using JetBrains.Application.PluginSupport;
using JetBrains.DataFlow;
using JetBrains.Util;

namespace KaVE.VsFeedbackGenerator.Utils
{
    [ShellComponent]
    class ExtensionSettingsListener
    {
        private const string Id = "KaVE Feedback Generator";
        private readonly PluginsDirectory _directory;

        public ExtensionSettingsListener(Lifetime lifetime, PluginsDirectory directory)
        {
            _directory = directory;
            RegisterPluginUnloadCallback(lifetime);
        }

        private void RegisterPluginUnloadCallback(Lifetime lifetime)
        {
            var plugin = GetPlugin(Id);
            if (plugin != null)
            {
                plugin.IsEnabled.BeforeChange.Advise(lifetime, BeforePluginStateChangedEvent);
            }
        }

        private Plugin GetPlugin(string id)
        {
            var feedbackManagerPlugin = _directory.Plugins.ToList().Find(plugin => plugin.ID == id);
            return feedbackManagerPlugin;
        }

        private void BeforePluginStateChangedEvent(BeforePropertyChangedEventArgs<bool?> changedEvent)
        {
            if (IsDisabled(changedEvent))
            {
                MessageBox.ShowExclamation("Sie müssen ihre Entwicklungsumgebung neustarten, damit das Plugin weiterhin korrekt funktioniert!");
            }
        }

        private static bool IsDisabled(BeforePropertyChangedEventArgs<bool?> changedEvent)
        {
            return changedEvent.HasNew && changedEvent.New == false;
        }
    }
}

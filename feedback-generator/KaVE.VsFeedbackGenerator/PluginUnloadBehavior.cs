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
using KaVE.VsFeedbackGenerator.Properties;

namespace KaVE.VsFeedbackGenerator
{
    [ShellComponent]
    class PluginUnloadBehavior
    {
        private const string PluginId = "KaVE Feedback Generator";
        private readonly PluginsDirectory _directory;

        public PluginUnloadBehavior(Lifetime lifetime, PluginsDirectory directory)
        {
            _directory = directory;
            RegisterPluginUnloadCallback(lifetime);
        }

        private void RegisterPluginUnloadCallback(Lifetime lifetime)
        {
            var plugin = GetPlugin(PluginId);
            if (plugin != null)
            {
                plugin.IsEnabled.BeforeChange.Advise(lifetime, BeforePluginStateChangedEvent);
            }
        }

        private Plugin GetPlugin(string id)
        {
            return _directory.Plugins.ToList().Find(plugin => plugin.ID == id);
        }

        private static void BeforePluginStateChangedEvent(BeforePropertyChangedEventArgs<bool?> changedEvent)
        {
            if (IsDisabled(changedEvent))
            {
                MessageBox.ShowExclamation(General.DisableWarning);
            }
        }

        private static bool IsDisabled(BeforePropertyChangedEventArgs<bool?> changedEvent)
        {
            return changedEvent.HasNew && changedEvent.New == false;
        }
    }
}

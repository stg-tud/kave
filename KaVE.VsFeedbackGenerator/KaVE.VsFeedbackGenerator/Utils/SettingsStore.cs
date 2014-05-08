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
using JetBrains.Application;
using JetBrains.Application.DataContext;
using JetBrains.Application.Settings;
using RSISettingsStore = JetBrains.Application.Settings.ISettingsStore;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public interface ISettingsStore
    {
        TSettingsInstance GetSettings<TSettingsInstance>();
        void SetSettings<TSettingsType>(TSettingsType settingsInstance) where TSettingsType : class;
    }

    [ShellComponent]
    public class SettingsStore : ISettingsStore
    {
        private readonly RSISettingsStore _settingsStore;
        private readonly DataContexts _dataContexts;

        public SettingsStore(RSISettingsStore settingsStore, DataContexts dataContexts)
        {
            _settingsStore = settingsStore;
            _dataContexts = dataContexts;
        }

        private IContextBoundSettingsStore ContextStore
        {
            get
            {
                var contextRange = ContextRange.Smart((lt, _) => _dataContexts.Empty);
                return _settingsStore.BindToContextTransient(contextRange);
            }
        }

        public TSettingsInstance GetSettings<TSettingsInstance>()
        {
            return ContextStore.GetKey<TSettingsInstance>(SettingsOptimization.DoMeSlowly);
        }

        public void SetSettings<TSettingsType>(TSettingsType settingsInstance) where TSettingsType : class
        {
            ContextStore.SetKey(settingsInstance, SettingsOptimization.DoMeSlowly);
        }
    }
}
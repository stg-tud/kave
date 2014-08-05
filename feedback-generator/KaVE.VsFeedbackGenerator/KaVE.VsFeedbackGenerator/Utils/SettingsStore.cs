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
 *    - Uli Fahrer
 */
using System;
using System.Reflection;
using JetBrains.Application;
using JetBrains.Application.DataContext;
using JetBrains.Application.Settings;
using JetBrains.Extension;
using KaVE.Utils.Assertion;
using KaVE.Utils.Reflection;
using RSISettingsStore = JetBrains.Application.Settings.ISettingsStore;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public interface ISettingsStore
    {
        TSettingsType GetSettings<TSettingsType>() where TSettingsType : class, new();
        void SetSettings<TSettingsType>(TSettingsType settingsInstance) where TSettingsType : class, new();
        void UpdateSettings<TSettingsType>(Action<TSettingsType> update) where TSettingsType : class, new();
        void ResetSettings<TSettingsType>() where TSettingsType : class, new();
    }

    [ShellComponent]
    public class SettingsStore : ISettingsStore
    {
        private readonly RSISettingsStore _settingsStore;
        private readonly DataContexts _dataContexts;
        private readonly ISettingsOptimization _optimization;

        public SettingsStore(RSISettingsStore settingsStore, DataContexts dataContexts, ISettingsOptimization optimization)
        {
            _settingsStore = settingsStore;
            _dataContexts = dataContexts;
            _optimization = optimization;
        }

        private IContextBoundSettingsStore ContextStore
        {
            get
            {
                var contextRange = ContextRange.Smart((lt, _) => _dataContexts.Empty);
                return _settingsStore.BindToContextTransient(contextRange);
            }
        }

        public TSettingsType GetSettings<TSettingsType>() where TSettingsType : class, new()
        {
            return ContextStore.GetKey<TSettingsType>(_optimization);
        }

        public void SetSettings<TSettingsType>(TSettingsType settingsInstance) where TSettingsType : class, new()
        {
            ContextStore.SetKey(settingsInstance, _optimization);
        }

        public void UpdateSettings<TSettingsType>(Action<TSettingsType> update) where TSettingsType : class, new()
        {
            var settings = GetSettings<TSettingsType>();
            update(settings);
            SetSettings(settings);
        }

        public void ResetSettings<TSettingsType>() where TSettingsType : class, new()
        {
            var defaultInstance = CreateDefaultInstance<TSettingsType>();
            SetSettings(defaultInstance);
        }

        internal static TSettings CreateDefaultInstance<TSettings>() where TSettings : class, new()
        {
            var settings = new TSettings();
            var settingsEntryMembers = typeof(TSettings).GetMembersWithCustomAttributeNoInherit<SettingsEntryAttribute>();
            foreach (var settingsEntryMember in settingsEntryMembers)
            {
                var attribute = settingsEntryMember.GetCustomAttributeNoInherit<SettingsEntryAttribute>();
                var settingsField = settingsEntryMember as FieldInfo;
                if (settingsField != null)
                {
                    var fieldType = settingsField.FieldType;
                    var defaultValue = GetDefaultValue(attribute, fieldType);
                    settingsField.SetValue(settings, defaultValue);
                    continue;
                }
                var settingsProperty = settingsEntryMember as PropertyInfo;
                if (settingsProperty != null)
                {
                    var propertyType = settingsProperty.PropertyType;
                    var defaultValue = GetDefaultValue(attribute, propertyType);
                    settingsProperty.SetValue(settings, defaultValue, new object[0]);
                    continue;
                }
                Asserts.Fail("unhandled settings-member type: {0}", settingsEntryMember.GetType());
            }

            return settings;
        }

        private static object GetDefaultValue(SettingsEntryAttribute attribute, Type settingsType)
        {
            var defaultValue = attribute.DefaultValue;
            if (defaultValue != null)
            {
                var defaultValueType = defaultValue.GetType();
                if (settingsType == typeof (DateTime) && defaultValueType == typeof (string))
                {
                    defaultValue = DateTime.Parse((string) defaultValue);
                }
                if (settingsType == typeof(TimeSpan) && defaultValueType == typeof(string))
                {
                    defaultValue = TimeSpan.Parse((string)defaultValue);
                }
            }
            return defaultValue;
        }
    }
}

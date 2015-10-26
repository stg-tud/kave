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
using JetBrains.Application;
using KaVE.Commons.Utils.CodeCompletion.Impl;
using KaVE.Commons.Utils.IO;
using KaVE.JetBrains.Annotations;
using KaVE.RS.Commons.Settings;
using KaVE.RS.Commons.Settings.KaVE.RS.Commons.Settings;

namespace KaVE.RS.Commons.Injectables
{
    [ShellComponent]
    internal class InjectableUsageModelsSource : UsageModelsSource
    {
        public InjectableUsageModelsSource([NotNull] IIoUtils ioUtils, [NotNull] ISettingsStore store)
            : base(ioUtils, ConvertToUri(store.GetSettings<ModelStoreSettings>().ModelStoreUri))
        {
            store.SettingsChanged += (sender, args) =>
            {
                if (args.SettingsType == typeof (ModelStoreSettings))
                {
                    Source = ConvertToUri(store.GetSettings<ModelStoreSettings>().ModelStoreUri);
                }
            };
        }

        [CanBeNull]
        private static Uri ConvertToUri(string uriString)
        {
            try
            {
                return new Uri(uriString);
            }
            catch (UriFormatException)
            {
                return null;
            }
        }
    }
}
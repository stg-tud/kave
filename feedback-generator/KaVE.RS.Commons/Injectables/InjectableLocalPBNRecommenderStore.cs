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
using KaVE.Commons.Utils.CodeCompletion.Impl;
using KaVE.Commons.Utils.CodeCompletion.Impl.Stores;
using KaVE.Commons.Utils.CodeCompletion.Impl.Stores.UsageModelSources;
using KaVE.Commons.Utils.CodeCompletion.Stores;
using KaVE.Commons.Utils.IO;
using KaVE.JetBrains.Annotations;
using KaVE.RS.Commons.Settings;
using KaVE.RS.Commons.Settings.KaVE.RS.Commons.Settings;

namespace KaVE.RS.Commons.Injectables
{
    [ShellComponent]
    public class InjectableLocalPBNRecommenderStore : SmilePBNRecommenderStore
    {
        public InjectableLocalPBNRecommenderStore([NotNull] ISettingsStore store,
            [NotNull] IIoUtils io,
            [NotNull] ITypePathUtil typePathUtil,
            [NotNull] IRemotePBNRecommenderStore remoteStore)
            : base(
                new FilePathLocalUsageModelsSource(
                    store.GetSettings<ModelStoreSettings>().ModelStorePath,
                    io,
                    typePathUtil),
                remoteStore)
        {
            store.SettingsChanged += (sender, args) =>
            {
                if (args.SettingsType == typeof (ModelStoreSettings))
                {
                    ModelsSource =
                        new FilePathLocalUsageModelsSource(
                            store.GetSettings<ModelStoreSettings>().ModelStorePath,
                            io,
                            typePathUtil);
                }
            };
        }
    }
}
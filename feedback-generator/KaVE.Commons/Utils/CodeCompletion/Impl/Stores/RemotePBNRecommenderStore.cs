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

using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.CodeCompletion.Impl.Stores.UsageModelSources;
using KaVE.Commons.Utils.CodeCompletion.Stores;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Utils.CodeCompletion.Impl.Stores
{
    public class RemotePBNRecommenderStore : IRemotePBNRecommenderStore
    {
        [CanBeNull]
        protected string LocalPath;

        [NotNull]
        public IRemoteUsageModelsSource RemoteUsageModelsSource { get; protected set; }

        public RemotePBNRecommenderStore([NotNull] IRemoteUsageModelsSource remoteUsageModelsSource,
            [CanBeNull] string localPath)
        {
            LocalPath = localPath;
            RemoteUsageModelsSource = remoteUsageModelsSource;
        }

        public IEnumerable<UsageModelDescriptor> GetAvailableModels()
        {
            return GetModelsDictionary().Values;
        }

        public bool IsAvailable(CoReTypeName type)
        {
            return GetModelsDictionary().ContainsKey(type);
        }

        public void Load(CoReTypeName type)
        {
            try
            {
                RemoteUsageModelsSource.Load(GetModelsDictionary()[type], LocalPath);
            }
            catch (KeyNotFoundException) {}
        }

        private IDictionary<CoReTypeName, UsageModelDescriptor> GetModelsDictionary()
        {
            var availableModels = new Dictionary<CoReTypeName, UsageModelDescriptor>();

            foreach (
                var newModel in
                    RemoteUsageModelsSource.GetUsageModels().Where(
                        newModel =>
                            !availableModels.ContainsKey(newModel.TypeName) ||
                            newModel.Version > availableModels[newModel.TypeName].Version))
            {
                availableModels[newModel.TypeName] = newModel;
            }

            return availableModels;
        }
    }
}
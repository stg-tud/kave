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
    public class SmilePBNRecommenderStore : ILocalPBNRecommenderStore
    {
        public ILocalUsageModelsSource ModelsSource { get; protected set; }

        public bool EnableAutoRemoteLoad = false;

        private readonly IRemotePBNRecommenderStore _remoteStore;

        public SmilePBNRecommenderStore([NotNull] ILocalUsageModelsSource modelsSource,
            [NotNull] IRemotePBNRecommenderStore remoteStore)
        {
            _remoteStore = remoteStore;
            ModelsSource = modelsSource;
        }

        [Pure]
        public IEnumerable<UsageModelDescriptor> GetAvailableModels()
        {
            return GetModelsDictionary().Values;
        }

        private IDictionary<CoReTypeName, UsageModelDescriptor> GetModelsDictionary()
        {
            var availableModels = new Dictionary<CoReTypeName, UsageModelDescriptor>();

            foreach (
                var newModel in
                    ModelsSource.GetUsageModels().Where(
                        newModel =>
                            !availableModels.ContainsKey(newModel.TypeName) ||
                            newModel.Version > availableModels[newModel.TypeName].Version))
            {
                availableModels[newModel.TypeName] = newModel;
            }

            return availableModels;
        }

        [Pure]
        public bool IsAvailable(CoReTypeName type)
        {
            return GetModelsDictionary().ContainsKey(type);
        }

        public IPBNRecommender Load(CoReTypeName type)
        {
            var recommender = ModelsSource.Load(type);
            if (recommender == null)
            {
                LoadFromRemote(type);
            }

            return recommender;
        }

        public void Remove(CoReTypeName type)
        {
            ModelsSource.Remove(type);
        }

        private void LoadFromRemote(CoReTypeName type)
        {
            if (EnableAutoRemoteLoad)
            {
                _remoteStore.Load(type);
            }
        }
    }
}
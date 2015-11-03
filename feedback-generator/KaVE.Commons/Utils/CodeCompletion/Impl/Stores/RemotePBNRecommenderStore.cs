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
using KaVE.Commons.Utils.CodeCompletion.Stores;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Utils.CodeCompletion.Impl.Stores
{
    public class RemotePBNRecommenderStore : IRemotePBNRecommenderStore
    {
        [CanBeNull]
        protected string LocalPath;

        [NotNull]
        public IUsageModelsSource UsageModelsSource { get; protected set; }

        [NotNull]
        private readonly Dictionary<CoReTypeName, UsageModelDescriptor> _availableModels =
            new Dictionary<CoReTypeName, UsageModelDescriptor>();

        public RemotePBNRecommenderStore([NotNull] IUsageModelsSource usageModelsSource, [CanBeNull] string localPath)
        {
            LocalPath = localPath;
            UsageModelsSource = usageModelsSource;
            ReloadAvailableModels();
        }

        public void ReloadAvailableModels()
        {
            _availableModels.Clear();

            try
            {
                foreach (
                    var newModel in
                        UsageModelsSource.GetUsageModels().Where(
                            newModel =>
                                !_availableModels.ContainsKey(newModel.TypeName) ||
                                newModel.Version > _availableModels[newModel.TypeName].Version))
                {
                    _availableModels[newModel.TypeName] = newModel;
                }
            }
            catch
            {
                _availableModels.Clear();
            }
        }

        public bool IsAvailable(CoReTypeName typeName)
        {
            return _availableModels.ContainsKey(typeName);
        }

        public void Load(CoReTypeName typeName)
        {
            try
            {
                UsageModelsSource.Load(_availableModels[typeName], LocalPath);
            }
            catch (KeyNotFoundException) {}
        }

        public void LoadAll()
        {
            foreach (var model in GetAvailableModels())
            {
                Load(model.TypeName);
            }
        }

        public IEnumerable<UsageModelDescriptor> GetAvailableModels()
        {
            return _availableModels.Values;
        }
    }
}
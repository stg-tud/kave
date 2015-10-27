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
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Utils.CodeCompletion.Impl
{
    public interface IRemotePBNRecommenderStore
    {
        bool IsAvailable([NotNull] CoReTypeName typeName);

        void Load([NotNull] CoReTypeName typeName);

        [NotNull]
        IEnumerable<UsageModelDescriptor> GetAvailableModels();

        void ReloadAvailableModels();
    }

    public class RemotePBNRecommenderStore : IRemotePBNRecommenderStore
    {
        [CanBeNull]
        protected string LocalPath;

        [NotNull]
        protected IUsageModelsSource UsageModelsSource;

        [NotNull]
        private readonly Dictionary<CoReTypeName, UsageModelDescriptor> _availableModels =
            new Dictionary<CoReTypeName, UsageModelDescriptor>();

        public RemotePBNRecommenderStore([NotNull] IUsageModelsSource usageModelsSource, [CanBeNull] string localPath)
        {
            LocalPath = localPath;
            UsageModelsSource = usageModelsSource;
        }

        public void ReloadAvailableModels()
        {
            _availableModels.Clear();

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

        public IEnumerable<UsageModelDescriptor> GetAvailableModels()
        {
            return _availableModels.Values;
        }
    }
}
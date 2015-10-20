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
using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Assertion;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Utils.CodeCompletion.Impl
{
    public interface IRemotePBNRecommenderStore
    {
        bool IsAvailable([NotNull] CoReTypeName typeName);

        void Load([NotNull] CoReTypeName typeName);

        [NotNull]
        IEnumerable<UsageModelDescriptor> GetAvailableModels();
    }
    
    public class RemotePBNRecommenderStore : IRemotePBNRecommenderStore
    {
        private readonly IUsageModelsSource _usageModelsSource;

        public RemotePBNRecommenderStore([NotNull] IUsageModelsSource usageModelsSource)
        {
            _usageModelsSource = usageModelsSource;
        }

        public bool IsAvailable(CoReTypeName typeName)
        {
            return _usageModelsSource.UsageModels.Any(modelDescription => typeName.Equals(modelDescription.TypeName));
        }

        public void Load(CoReTypeName typeName)
        {
            Asserts.That(IsAvailable(typeName));
            _usageModelsSource.LoadZip(typeName);
        }

        public IEnumerable<UsageModelDescriptor> GetAvailableModels()
        {
            var availableModels = new Dictionary<CoReTypeName, UsageModelDescriptor>();

            foreach (
                var newModel in
                    _usageModelsSource.UsageModels.Where(
                        newModel =>
                            !availableModels.ContainsKey(newModel.TypeName) ||
                            newModel.Version > availableModels[newModel.TypeName].Version))
            {
                availableModels[newModel.TypeName] = newModel;
            }

            return availableModels.Values;
        }
    }
}
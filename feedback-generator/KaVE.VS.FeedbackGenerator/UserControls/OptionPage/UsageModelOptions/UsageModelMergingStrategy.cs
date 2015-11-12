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
using JetBrains.Application;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.CodeCompletion.Stores;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.VS.FeedbackGenerator.UserControls.OptionPage.UsageModelOptions
{
    public interface IUsageModelMergingStrategy
    {
        IEnumerable<IUsageModelsTableRow> MergeAvailableModels([CanBeNull] ILocalPBNRecommenderStore localStore,
            [CanBeNull] IRemotePBNRecommenderStore remoteStore);
    }

    [ShellComponent]
    public class UsageModelMergingStrategy : IUsageModelMergingStrategy
    {
        [Pure]
        public IEnumerable<IUsageModelsTableRow> MergeAvailableModels(ILocalPBNRecommenderStore localStore,
            IRemotePBNRecommenderStore remoteStore)
        {
            var mergedModels = remoteStore != null
                ? remoteStore.GetAvailableModels()
                             .ToDictionary(
                                 remoteModel => remoteModel.TypeName,
                                 remoteModel =>
                                     new UsageModelsTableRow(
                                         localStore,
                                         remoteStore,
                                         remoteModel.TypeName,
                                         null,
                                         remoteModel.Version))
                : new Dictionary<CoReTypeName, UsageModelsTableRow>();

            foreach (
                var localModel in
                    localStore != null ? localStore.GetAvailableModels() : Lists.NewList<UsageModelDescriptor>())
            {
                if (mergedModels.ContainsKey(localModel.TypeName))
                {
                    mergedModels[localModel.TypeName].LoadedVersion = localModel.Version;
                }
                else
                {
                    mergedModels.Add(
                        localModel.TypeName,
                        new UsageModelsTableRow(localStore, remoteStore, localModel.TypeName, localModel.Version, null));
                }
            }

            return mergedModels.Values;
        }
    }
}
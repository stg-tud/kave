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
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.CodeCompletion.Stores;
using KaVE.Commons.Utils.Collections;
using KaVE.VS.FeedbackGenerator.UserControls.OptionPage.UsageModelOptions;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.OptionPage.UsageModelOptions
{
    internal class UsageModelMergingStrategyTest
    {
        private static IEnumerable<UsageModelDescriptor> SomeLocalModels
        {
            get
            {
                return new KaVEList<UsageModelDescriptor>
                {
                    new UsageModelDescriptor(new CoReTypeName("LSomeAssembly/SomeType"), 2),
                    new UsageModelDescriptor(new CoReTypeName("LSomeOtherAssembly/OnlyLocalType"), 3)
                };
            }
        }

        private static IEnumerable<UsageModelDescriptor> SomeRemoteModels
        {
            get
            {
                return new KaVEList<UsageModelDescriptor>
                {
                    new UsageModelDescriptor(new CoReTypeName("LSomeAssembly/SomeType"), 4),
                    new UsageModelDescriptor(new CoReTypeName("LSomeAssembly/OnlyRemoteType"), 5)
                };
            }
        }

        private static IEnumerable<IUsageModelsTableRow> ExpectedRows
        {
            get
            {
                return new KaVEList<IUsageModelsTableRow>
                {
                    new UsageModelsTableRow(null, null, new CoReTypeName("LSomeAssembly/SomeType"), 2, 4),
                    new UsageModelsTableRow(null, null, new CoReTypeName("LSomeOtherAssembly/OnlyLocalType"), 3, null),
                    new UsageModelsTableRow(null, null, new CoReTypeName("LSomeAssembly/OnlyRemoteType"), null, 5)
                };
            }
        }

        [Test]
        public void MergeTest()
        {
            var localStore = Mock.Of<ILocalPBNRecommenderStore>();
            Mock.Get(localStore).Setup(store => store.GetAvailableModels()).Returns(SomeLocalModels);
            var remoteStore = Mock.Of<IRemotePBNRecommenderStore>();
            Mock.Get(remoteStore).Setup(store => store.GetAvailableModels()).Returns(SomeRemoteModels);

            CollectionAssert.AreEquivalent(
                ExpectedRows,
                new UsageModelMergingStrategy().MergeAvailableModels(localStore, remoteStore));
        }
    }
}
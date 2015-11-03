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
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.CodeCompletion.Stores;
using KaVE.Commons.Utils.Json;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.CodeCompletion.Stores
{
    internal class UsageModelsSourceBaseTest
    {
        private UsageModelsSourceBaseTestImpl _uut;

        [SetUp]
        public void Setup()
        {
            _uut = new UsageModelsSourceBaseTestImpl();
        }

        [Test]
        public void ShouldShowNoModelsForEmptyFile()
        {
            SetIndexContent("");
            Assert.IsEmpty(_uut.GetUsageModels());
        }

        [Test]
        public void ShouldShowNoModelsForInvalidFile()
        {
            SetIndexContent("some invalid content");
            Assert.IsEmpty(_uut.GetUsageModels());
        }

        [Test]
        public void ShouldShowModelsForValidFile()
        {
            var expectedModels = new List<UsageModelDescriptor>
            {
                new UsageModelDescriptor(new CoReTypeName("LSomeType"), 3),
                new UsageModelDescriptor(new CoReTypeName("LOtherSomeType"), 4),
                new UsageModelDescriptor(new CoReTypeName("LSystem/SomeType"), 5)
            };
            SetIndexContent(expectedModels.ToCompactJson());

            CollectionAssert.AreEquivalent(expectedModels, _uut.GetUsageModels());
        }

        private void SetIndexContent(string newContent)
        {
            _uut.IndexContent = newContent;
        }

        private class UsageModelsSourceBaseTestImpl : UsageModelsSourceBase
        {
            public string IndexContent;

            public override void Load(UsageModelDescriptor model, string baseTargetDirectory)
            {
                throw new NotImplementedException();
            }

            protected override string GetIndexContent()
            {
                return IndexContent;
            }
        }
    }
}
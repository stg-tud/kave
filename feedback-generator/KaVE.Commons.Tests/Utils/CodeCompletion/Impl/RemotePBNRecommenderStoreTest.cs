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
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.CodeCompletion;
using KaVE.Commons.Utils.CodeCompletion.Impl;
using Moq;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.CodeCompletion.Impl
{
    internal class RemotePBNRecommenderStoreTest
    {
        #region helper

        private static CoReTypeName SomeType
        {
            get { return new CoReTypeName("LSomePackage/SomeType"); }
        }

        private static CoReTypeName SomeOtherType
        {
            get { return new CoReTypeName("LSomePackage/SomeOtherType"); }
        }

        private static IEnumerable<UsageModelDescriptor> AvailableModels
        {
            get
            {
                return new List<UsageModelDescriptor>
                {
                    new UsageModelDescriptor(SomeType, 1),
                    new UsageModelDescriptor(SomeOtherType, 1),
                    new UsageModelDescriptor(SomeOtherType, 2)
                };
            }
        }

        private static IEnumerable<UsageModelDescriptor> NewestAvailableModels
        {
            get
            {
                return new List<UsageModelDescriptor>
                {
                    new UsageModelDescriptor(SomeType, 1),
                    new UsageModelDescriptor(SomeOtherType, 2)
                };
            }
        }

        #endregion

        private IUsageModelsSource _testSource;
        private RemotePBNRecommenderStore _sut;

        [SetUp]
        public void SetUp()
        {
            _testSource = Mock.Of<IUsageModelsSource>();
            Mock.Get(_testSource).Setup(source => source.UsageModels).Returns(AvailableModels);

            _sut = new RemotePBNRecommenderStore(_testSource);
        }

        [Test]
        public void IsAvailable_HappyPath()
        {
            Assert.IsTrue(_sut.IsAvailable(SomeType));
        }

        [Test]
        public void IsAvailable_NotAvailable()
        {
            Assert.IsFalse(_sut.IsAvailable(new CoReTypeName("LNotAvailable")));
        }

        [Test]
        public void Load_HappyPath()
        {
            _sut.Load(SomeType);
            Mock.Get(_testSource)
                .Verify(source => source.LoadZip(It.Is<CoReTypeName>(typeName => Equals(SomeType, typeName))));
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void Load_NotAvailable()
        {
            _sut.Load(new CoReTypeName("LNotAvailable"));
        }

        [Test]
        public void ShouldProvideNewestAvailableModels()
        {
            Assert.AreEqual(NewestAvailableModels, _sut.GetAvailableModels());
        }
    }
}
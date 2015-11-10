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

using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.CodeCompletion;
using KaVE.Commons.Utils.CodeCompletion.Impl.Stores;
using KaVE.Commons.Utils.CodeCompletion.Impl.Stores.UsageModelSources;
using KaVE.Commons.Utils.CodeCompletion.Stores;
using Moq;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.CodeCompletion.Impl.Stores
{
    internal class SmilePBNRecommenderStoreTest
    {
        private ILocalUsageModelsSource _modelsSource;
        private IRemotePBNRecommenderStore _remoteStore;
        private SmilePBNRecommenderStore _sut;

        private static readonly CoReTypeName SomeType = new CoReTypeName("LSomeType");
        private static readonly IPBNRecommender TestRecommender = Mock.Of<IPBNRecommender>();
        private UsageModelDescriptor[] _testModels;

        [SetUp]
        public void SetUp()
        {
            _remoteStore = Mock.Of<IRemotePBNRecommenderStore>();

            _testModels = new[]
            {
                new UsageModelDescriptor(SomeType, 1),
                new UsageModelDescriptor(new CoReTypeName("LSomeOtherType"), 2)
            };
            _modelsSource = Mock.Of<ILocalUsageModelsSource>();
            Mock.Get(_modelsSource).Setup(source => source.GetUsageModels()).Returns(() => _testModels);
            Mock.Get(_modelsSource).Setup(source => source.Load(SomeType)).Returns(TestRecommender);

            _sut = new SmilePBNRecommenderStore(_modelsSource, _remoteStore);
        }

        [Test]
        public void ShouldTriggerRemoteLoadOnNonAvailableModels()
        {
            _sut.EnableAutoRemoteLoad = true;
            var unavailableType = new CoReTypeName("LNotAvailable");

            _sut.Load(unavailableType);

            Mock.Get(_remoteStore).Verify(remote => remote.Load(unavailableType), Times.Once);
        }

        [Test]
        public void ShouldNotTriggerRemoteLoadOnNonAvailableModelsIfDisabled()
        {
            _sut.EnableAutoRemoteLoad = false;
            var unavailableType = new CoReTypeName("LNotAvailable");

            _sut.Load(unavailableType);

            Mock.Get(_remoteStore).Verify(remote => remote.Load(unavailableType), Times.Never);
        }

        [Test]
        public void ShouldGetModelsFromSource()
        {
            CollectionAssert.AreEquivalent(_testModels, _sut.GetAvailableModels());
        }

        [Test]
        public void ShouldOnlyShowNewestModels()
        {
            _testModels = new[]
            {
                new UsageModelDescriptor(SomeType, 1),
                new UsageModelDescriptor(SomeType, 2)
            };

            var expected = new[] {new UsageModelDescriptor(SomeType, 2)};
            CollectionAssert.AreEquivalent(expected, _sut.GetAvailableModels());
        }

        [Test]
        public void IsAvailableTest()
        {
            Assert.IsTrue(_sut.IsAvailable(SomeType));
            Assert.IsFalse(_sut.IsAvailable(new CoReTypeName("LNotAvailable")));
        }

        [Test]
        public void ShouldLoadFromLocalSource()
        {
            Assert.AreSame(_modelsSource.Load(SomeType), _sut.Load(SomeType));
        }

        [Test]
        public void ShouldRemoveUsingLocalSource()
        {
            _sut.Remove(SomeType);
            Mock.Get(_modelsSource).Verify(source => source.Remove(SomeType), Times.Once);
        }
    }
}
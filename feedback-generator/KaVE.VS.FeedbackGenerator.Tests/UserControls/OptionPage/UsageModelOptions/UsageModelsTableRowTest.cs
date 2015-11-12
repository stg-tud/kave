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
using KaVE.Commons.TestUtils;
using KaVE.Commons.Utils.CodeCompletion.Stores;
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.UserControls.OptionPage.UsageModelOptions;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.OptionPage.UsageModelOptions
{
    internal class UsageModelsTableRowTest
    {
        private static readonly CoReTypeName SomeTypeName = new CoReTypeName("LSomeAssembly/SomeType");
        private const int SomeLoadedVersion = 32;
        private const int SomeNewestAvailableVersion = 14;

        private UsageModelsTableRow _uut;
        private ILocalPBNRecommenderStore _localStore;
        private IRemotePBNRecommenderStore _remoteStore;

        [SetUp]
        public void Setup()
        {
            _localStore = Mock.Of<ILocalPBNRecommenderStore>();
            _remoteStore = Mock.Of<IRemotePBNRecommenderStore>();
            _uut = new UsageModelsTableRow(
                _localStore,
                _remoteStore,
                SomeTypeName,
                SomeLoadedVersion,
                SomeNewestAvailableVersion);
        }

        [TearDown]
        public void ClearRegistry()
        {
            Registry.Clear();
        }

        [TestCase(0, 1, true), TestCase(0, null, false), TestCase(1, 1, false)]
        public void IsUpdateableTest(int? localVersion, int? remoteVersion, bool shouldBeUpdateable)
        {
            _uut = new UsageModelsTableRow(_localStore, _remoteStore, SomeTypeName, localVersion, remoteVersion);
            Assert.AreEqual(shouldBeUpdateable, _uut.IsUpdateable);
        }

        [TestCase(null, 0, true), TestCase(null, null, false), TestCase(0, 1, false)]
        public void IsInstallableTest(int? localVersion, int? remoteVersion, bool shouldBeInstallable)
        {
            _uut = new UsageModelsTableRow(_localStore, _remoteStore, SomeTypeName, localVersion, remoteVersion);
            Assert.AreEqual(shouldBeInstallable, _uut.IsInstallable);
        }

        [TestCase(1, 1, true), TestCase(null, 0, false)]
        public void IsRemoveableTest(int? localVersion, int? remoteVersion, bool shouldBeRemoveable)
        {
            _uut = new UsageModelsTableRow(_localStore, _remoteStore, SomeTypeName, localVersion, remoteVersion);
            Assert.AreEqual(shouldBeRemoveable, _uut.IsRemoveable);
        }

        [Test]
        public void ToStringTest()
        {
            Assert.AreEqual("(LSomeType,0,-)", new UsageModelsTableRow(null, null, new CoReTypeName("LSomeType"), 0, null).ToString());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new UsageModelsTableRow(_localStore, _remoteStore, new CoReTypeName("LSomeType"), 1, 2);
            var b = new UsageModelsTableRow(_localStore, _remoteStore, new CoReTypeName("LSomeType"), 1, 2);
            Assert.AreEqual(a, b);
        }

        [Test]
        public void Equality_DifferentTypeNames()
        {
            var a = new UsageModelsTableRow(_localStore, _remoteStore, new CoReTypeName("LSomeType"), 1, 2);
            var b = new UsageModelsTableRow(_localStore, _remoteStore, new CoReTypeName("LSomeOtherType"), 1, 2);
            Assert.AreNotEqual(a, b);
        }

        [Test]
        public void Equality_DifferentLoadedVersions()
        {
            var a = new UsageModelsTableRow(_localStore, _remoteStore, new CoReTypeName("LSomeType"), 1, 2);
            var b = new UsageModelsTableRow(_localStore, _remoteStore, new CoReTypeName("LSomeType"), 2, 2);
            Assert.AreNotEqual(a, b);
        }

        [Test]
        public void Equality_DifferentNewestAvailableVersions()
        {
            var a = new UsageModelsTableRow(_localStore, _remoteStore, new CoReTypeName("LSomeType"), 1, 1);
            var b = new UsageModelsTableRow(_localStore, _remoteStore, new CoReTypeName("LSomeType"), 1, 2);
            Assert.AreNotEqual(a, b);
        }

        [Test]
        public void Equality_StoresAreIgnored()
        {
            var a = new UsageModelsTableRow(
                Mock.Of<ILocalPBNRecommenderStore>(),
                Mock.Of<IRemotePBNRecommenderStore>(),
                new CoReTypeName("LSomeType"),
                1,
                2);
            var b = new UsageModelsTableRow(
                Mock.Of<ILocalPBNRecommenderStore>(),
                Mock.Of<IRemotePBNRecommenderStore>(),
                new CoReTypeName("LSomeType"),
                1,
                2);
            Assert.AreEqual(a, b);
        }

        [Test]
        public void CallModelLoadOnLoad()
        {
            _uut.LoadModel();
            Mock.Get(_remoteStore).Verify(store => store.Load(SomeTypeName));
        }

        [Test]
        public void CallModelRemoveOnRemove()
        {
            _uut.RemoveModel();
            Mock.Get(_localStore).Verify(store => store.Remove(SomeTypeName));
        }

        [Test]
        public void DisableButtonsAfterFirstExecute()
        {
            _uut.LoadModel();
            _uut.RemoveModel();

            Assert.IsFalse(_uut.IsInstallable);
            Assert.IsFalse(_uut.IsUpdateable);
            Assert.IsFalse(_uut.IsRemoveable);
        }
    }
}
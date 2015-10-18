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
using KaVE.VS.FeedbackGenerator.UserControls.OptionPage.UsageModelOptions;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.OptionPage.UsageModelOptions
{
    internal class UsageModelsTableRowTest
    {
        private static readonly CoReTypeName SomeTypeName = new CoReTypeName("LSomeAssembly/SomeType");
        private const int SomeLoadedVersion = 32;
        private const int SomeNewestAvailableVersion = 14;

        private UsageModelsTableRow _uut;

        [SetUp]
        public void Setup()
        {
            _uut = new UsageModelsTableRow(
                SomeTypeName,
                SomeLoadedVersion,
                SomeNewestAvailableVersion);
        }

        [TestCase(0, 1, true)]
        [TestCase(0, null, false)]
        [TestCase(1, 1, false)]
        public void IsUpdateableTest(int? localVersion, int? remoteVersion, bool shouldBeUpdateable)
        {
            _uut = new UsageModelsTableRow(SomeTypeName, localVersion, remoteVersion);
            Assert.AreEqual(shouldBeUpdateable, _uut.IsUpdateable);
        }

        [TestCase(null, 0, true)]
        [TestCase(null, null, false)]
        [TestCase(0, 1, false)]
        public void IsInstallableTest(int? localVersion, int? remoteVersion, bool shouldBeInstallable)
        {
            _uut = new UsageModelsTableRow(SomeTypeName, localVersion, remoteVersion);
            Assert.AreEqual(shouldBeInstallable, _uut.IsInstallable);
        }

        [TestCase(1, 1, true)]
        [TestCase(null, 0, false)]
        public void IsRemoveableTest(int? localVersion, int? remoteVersion, bool shouldBeRemoveable)
        {
            _uut = new UsageModelsTableRow(SomeTypeName, localVersion, remoteVersion);
            Assert.AreEqual(shouldBeRemoveable, _uut.IsRemoveable);
        }
    }
}
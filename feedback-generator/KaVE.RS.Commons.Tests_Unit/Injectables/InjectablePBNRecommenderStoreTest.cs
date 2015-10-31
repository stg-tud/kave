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

using KaVE.Commons.Utils.CodeCompletion;
using KaVE.Commons.Utils.CodeCompletion.Impl;
using KaVE.Commons.Utils.CodeCompletion.Stores;
using KaVE.Commons.Utils.IO;
using KaVE.RS.Commons.Injectables;
using KaVE.RS.Commons.Settings;
using KaVE.RS.Commons.Settings.KaVE.RS.Commons.Settings;
using Moq;
using NUnit.Framework;

namespace KaVE.RS.Commons.Tests_Unit.Injectables
{
    internal class InjectablePBNRecommenderStoreTest
    {
        [Test]
        public void ShouldFetchModelStorePathFromSettings()
        {
            var settingsStore = Mock.Of<ISettingsStore>();
            Mock.Get(settingsStore).Setup(s => s.GetSettings<ModelStoreSettings>()).Returns(new ModelStoreSettings());
            var ioUtils = Mock.Of<IIoUtils>();

            // ReSharper disable once ObjectCreationAsStatement
            new InjectableLocalPBNRecommenderStore(
                ioUtils,
                settingsStore,
                new TypePathUtil(),
                Mock.Of<IRemotePBNRecommenderStore>());

            Mock.Get(settingsStore).Verify(s => s.GetSettings<ModelStoreSettings>());
        }

        [Test]
        public void ShouldUpdateBasePathOnceSettingChanges()
        {
            var settings = new ModelStoreSettings();
            var settingsStoreMock = new Mock<ISettingsStore>();
            settingsStoreMock.Setup(s => s.GetSettings<ModelStoreSettings>()).Returns(settings);

            var sut = new InjectableLocalPBNRecommenderStore(
                Mock.Of<IIoUtils>(),
                settingsStoreMock.Object,
                new TypePathUtil(),
                Mock.Of<IRemotePBNRecommenderStore>());

            Assert.AreEqual(settings.ModelStorePath, sut.BasePath);

            settings.ModelStorePath = @"C:\new\path";
            settingsStoreMock.Raise(
                s => s.SettingsChanged += null,
                new SettingsChangedEventArgs(typeof (ModelStoreSettings)));

            Assert.AreEqual(settings.ModelStorePath, sut.BasePath);
        }
    }
}
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
using KaVE.RS.Commons.Settings;
using KaVE.VS.FeedbackGenerator.Settings.ExportSettingsSuite;
using KaVE.VS.FeedbackGenerator.UserControls.OptionPage.GeneralOptions;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.OptionPage.GeneralOptions
{
    internal class GeneralOptionsViewModelTest
    {
        private const string TestUploadUrl = "http://foo.bar/";
        private const string TestUploadPrefix = "http://pre";

        private GeneralOptionsViewModel _uut;
        private List<string> _changedProperties;

        [SetUp]
        public void SetUp()
        {
            var settingsStore = Mock.Of<ISettingsStore>();
            Mock.Get(settingsStore).Setup(store => store.GetSettings<ExportSettings>()).Returns(
                new ExportSettings
                {
                    UploadUrl = TestUploadUrl,
                    WebAccessPrefix = TestUploadPrefix
                });

            _uut = new GeneralOptionsViewModel(settingsStore);

            _changedProperties = new List<string>();
            _uut.PropertyChanged += (sender, args) => { _changedProperties.Add(args.PropertyName); };
        }

        [Test]
        public void SetsInitialValuesOfPropertiesUsingExportSettings()
        {
            Assert.AreEqual(TestUploadUrl, _uut.UploadUrl);
            Assert.AreEqual(TestUploadPrefix, _uut.WebAccessPrefix);
        }

        [Test]
        public void PropertyChanged_UploadUrl()
        {
            _uut.UploadUrl = "new value";
            CollectionAssert.Contains(_changedProperties, "UploadUrl");
        }

        [Test]
        public void PropertyChanged_WebAccessPrefix()
        {
            _uut.WebAccessPrefix = "new value";
            CollectionAssert.Contains(_changedProperties, "WebAccessPrefix");
        }
    }
}
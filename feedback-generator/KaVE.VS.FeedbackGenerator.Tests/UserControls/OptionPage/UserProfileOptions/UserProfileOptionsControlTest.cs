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

using JetBrains.DataFlow;
using KaVE.RS.Commons.Settings;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.Settings.ExportSettingsSuite;
using KaVE.VS.FeedbackGenerator.UserControls.OptionPage.UserProfileOptions;
using KaVE.VS.FeedbackGenerator.UserControls.UserProfile;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.OptionPage.UserProfileOptions
{
    [RequiresSTA]
    internal class UserProfileOptionsControlTest : BaseUserControlTest
    {
        private Mock<ISettingsStore> _mockSettingsStore;
        private readonly Lifetime _lifetime = EternalLifetime.Instance;

        [SetUp]
        public void SetUp()
        {
            _mockSettingsStore = new Mock<ISettingsStore>();
            _mockSettingsStore.Setup(settingStore => settingStore.GetSettings<ExportSettings>())
                              .Returns(new ExportSettings());
            _mockSettingsStore.Setup(settingStore => settingStore.GetSettings<UserProfileSettings>())
                              .Returns(new UserProfileSettings());
        }

        private UserProfileOptionsControl Open()
        {
            return
                OpenWindow(
                    new UserProfileOptionsControl(
                        _lifetime,
                        null,
                        _mockSettingsStore.Object,
                        null,
                        null,
                        null));
        }

        [Test]
        public void DataContextIsSetCorrectly()
        {
            var sut = Open();
            Assert.IsInstanceOf<UserProfileContext>(sut.DataContext);
        }
    }
}
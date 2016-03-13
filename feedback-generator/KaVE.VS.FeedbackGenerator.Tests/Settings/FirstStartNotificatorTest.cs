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

using KaVE.RS.Commons.Settings;
using KaVE.VS.FeedbackGenerator.Settings;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Settings
{
    [RequiresSTA]
    internal class FirstStartNotificatorTest
    {
        private KaVESettings _settings;
        private ISettingsStore _settingsStore;

        [SetUp]
        public void Setup()
        {
            _settings = new KaVESettings();

            _settingsStore = Mock.Of<ISettingsStore>();
            Mock.Get(_settingsStore).Setup(ss => ss.GetSettings<KaVESettings>()).Returns(_settings);
        }

        [Test]
        public void SettingIsSetOnFirstStart()
        {
            _settings.IsFirstStart = true;
            // ReSharper disable once ObjectCreationAsStatement
            new FirstStartNotificator(_settingsStore);
            Mock.Get(_settingsStore).Verify(ss => ss.SetSettings(_settings));
            Assert.False(_settings.IsFirstStart);
        }

        [Test]
        public void SettingIsNotChangedOnSecondStart()
        {
            _settings.IsFirstStart = false;
            // ReSharper disable once ObjectCreationAsStatement
            new FirstStartNotificator(_settingsStore);
            Mock.Get(_settingsStore).Verify(ss => ss.SetSettings(_settings), Times.Never);
            Assert.False(_settings.IsFirstStart);
        }
    }
}
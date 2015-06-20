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

using JetBrains.Application.DataContext;
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.SessionManager;
using KaVE.VS.FeedbackGenerator.SessionManager.Presentation;
using KaVE.VS.FeedbackGenerator.Utils;
using KaVE.VS.FeedbackGenerator.Utils.Logging;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Utils
{
    internal class SettingsCleanerTest
    {
        private SettingsCleaner _uut;
        private Mock<ISettingsStore> _mockSettingsStore;
        private Mock<ILogManager> _mockLogFileManager;

        [SetUp]
        public void SetUp()
        {
            _mockSettingsStore = new Mock<ISettingsStore>();
            _mockLogFileManager = new Mock<ILogManager>();
            Registry.RegisterComponent(_mockSettingsStore.Object);
            Registry.RegisterComponent(_mockLogFileManager.Object);

            _uut = new SettingsCleaner();
        }

        [TearDown]
        public void TearDown()
        {
            Registry.Clear();
        }

        [Test]
        public void SettingsWillBeRestoredToDefaultValue()
        {
            _uut.Execute(new Mock<IDataContext>().Object, null);

            _mockSettingsStore.Verify(s => s.ResetSettings<UploadSettings>());
            _mockSettingsStore.Verify(s => s.ResetSettings<ExportSettings>());
            _mockSettingsStore.Verify(s => s.ResetSettings<FeedbackSettings>(), Times.Never);

            _mockLogFileManager.Verify(s => s.DeleteAllLogs());
        }
    }
}
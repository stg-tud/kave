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
 * 
 * Contributors:
 *    - Uli Fahrer
 */

using KaVE.VsFeedbackGenerator.SessionManager;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Json;
using KaVE.VsFeedbackGenerator.VsIntegration;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils
{
    [TestFixture]
    class SettingsRestoreTest
    {
        private SettingsRestore _uut;
        private Mock<ISettingsStore> _mockSettingsStore;
        private Mock<IDEEventLogFileManager> _mockLogFileManager;

        [SetUp]
        public void SetUp()
        {
            _mockSettingsStore = new Mock<ISettingsStore>();
            _mockLogFileManager = new Mock<IDEEventLogFileManager>();

            _uut = new SettingsRestore(_mockSettingsStore.Object, _mockLogFileManager.Object);
        }

        [Test]
        public void SettingsWillBeRestoredToDefaultValue()
        {
            _uut.RestoreDefaultSettings();

            _mockSettingsStore.Verify(s => s.ResetSettings<UploadSettings>());
            _mockSettingsStore.Verify(s => s.ResetSettings<ExportSettings>());
            _mockSettingsStore.Verify(s => s.ResetSettings<IDESessionSettings>());

            _mockLogFileManager.Verify(s => s.DeleteLogFileDirectory());
        }            
    }
}

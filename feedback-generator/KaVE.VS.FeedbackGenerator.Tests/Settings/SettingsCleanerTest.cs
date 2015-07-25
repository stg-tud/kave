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
using KaVE.RS.Commons.Settings;
using KaVE.RS.Commons.Settings.KaVE.RS.Commons.Settings;
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.Settings.ExportSettingsSuite;
using KaVE.VS.FeedbackGenerator.Utils.Logging;
using Moq;
using Moq.Language.Flow;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Settings
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
        public void GeneralSettingsWillBeRestored()
        {
            var dataContext = new Mock<IDataContext>();
            dataContext.Setup(datacontext => datacontext.GetData(SettingDataContextCreator.DataConstant)).Returns(new SettingResetType{ResetType = ResetTypes.GeneralSettings});
            _uut.Execute(dataContext.Object, null);

            _mockSettingsStore.Verify(s => s.ResetSettings<UploadSettings>());
            _mockSettingsStore.Verify(s => s.ResetSettings<ExportSettings>());
            _mockSettingsStore.Verify(s => s.ResetSettings<FeedbackSettings>(), Times.Never);
        }

        [Test]
        public void AnonymizationSettingsWillBeRestored()
        {
            var dataContext = new Mock<IDataContext>();
            dataContext.Setup(datacontext => datacontext.GetData(SettingDataContextCreator.DataConstant))
                       .Returns(new SettingResetType {ResetType = ResetTypes.AnonymizationSettings});
            _uut.Execute(dataContext.Object, null);

            _mockSettingsStore.Verify(s => s.ResetSettings<AnonymizationSettings>());
            _mockSettingsStore.Verify(s => s.ResetSettings<FeedbackSettings>(), Times.Never);
        }

        [Test]
        public void ModelStoreSettingsWillBeRestored()
        {
            var dataContext = new Mock<IDataContext>();
            dataContext.Setup(datacontext => datacontext.GetData(SettingDataContextCreator.DataConstant))
                       .Returns(new SettingResetType { ResetType = ResetTypes.ModelStoreSettings });
            _uut.Execute(dataContext.Object, null);

            _mockSettingsStore.Verify(s => s.ResetSettings<ModelStoreSettings>());
            _mockSettingsStore.Verify(s => s.ResetSettings<FeedbackSettings>(), Times.Never);
        }

        [Test]
        public void UserProfileSettingsWillBeRestored()
        {
            var dataContext = new Mock<IDataContext>();
            dataContext.Setup(datacontext => datacontext.GetData(SettingDataContextCreator.DataConstant))
                       .Returns(new SettingResetType {ResetType = ResetTypes.UserProfileSettings});
            _uut.Execute(dataContext.Object, null);

            _mockSettingsStore.Verify(s => s.ResetSettings<UserProfileSettings>());
            _mockSettingsStore.Verify(s => s.ResetSettings<FeedbackSettings>(), Times.Never);
        }

        [Test]
        public void FeedbackWillBeDeleted()
        {
            var dataContext = new Mock<IDataContext>();
            dataContext.Setup(datacontext => datacontext.GetData(SettingDataContextCreator.DataConstant))
                       .Returns(new SettingResetType {ResetType = ResetTypes.Feedback});
            _uut.Execute(dataContext.Object, null);

            _mockSettingsStore.Verify(s => s.ResetSettings<FeedbackSettings>(), Times.Never);

            _mockLogFileManager.Verify(s => s.DeleteAllLogs());
        }

    }
}
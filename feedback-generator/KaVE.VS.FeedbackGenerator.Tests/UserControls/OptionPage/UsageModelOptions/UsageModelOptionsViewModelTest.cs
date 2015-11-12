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
using System.Threading;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.CodeCompletion.Stores;
using KaVE.RS.Commons.Settings.KaVE.RS.Commons.Settings;
using KaVE.VS.FeedbackGenerator.Interactivity;
using KaVE.VS.FeedbackGenerator.Tests.Interactivity;
using KaVE.VS.FeedbackGenerator.UserControls.OptionPage.UsageModelOptions;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.OptionPage.UsageModelOptions
{
    public class UsageModelOptionsViewModelTest
    {
        private const string TestModelStorePath = @"c:/";
        private const string InvalidModelStorePath = @"c:/some/folder/that/surely/does/not/exist";
        
        private IEnumerable<IUsageModelsTableRow> _usageModelsTableContent;

        private UsageModelOptionsViewModel _uut;
        private InteractionRequestTestHelper<Notification> _notificationHelper;

        [SetUp]
        public void SetUp()
        {
            var rowMock = Mock.Of<IUsageModelsTableRow>();
            Mock.Get(rowMock).Setup(row => row.IsInstallable).Returns(true);
            Mock.Get(rowMock).Setup(row => row.IsUpdateable).Returns(true);
            Mock.Get(rowMock).Setup(row => row.IsRemoveable).Returns(true);
            _usageModelsTableContent = new List<IUsageModelsTableRow>
            {
                rowMock,
                rowMock,
                rowMock
            };

            var mergingStrategy = Mock.Of<IUsageModelMergingStrategy>();
            Mock.Get(mergingStrategy)
                .Setup(
                    strategy =>
                        strategy.MergeAvailableModels(
                            It.IsAny<ILocalPBNRecommenderStore>(),
                            It.IsAny<IRemotePBNRecommenderStore>()))
                .Returns(_usageModelsTableContent);

            _uut = new UsageModelOptionsViewModel(new ModelStoreSettings(), mergingStrategy, new KaVEBackgroundWorker());
            _notificationHelper = _uut.ErrorNotificationRequest.NewTestHelper();
        }

        [Test]
        public void ValidModelStoreInformationRaisesNoErrorNotification()
        {
            var info = _uut.ValidateModelStoreInformation(TestModelStorePath);
            Assert.IsFalse(_notificationHelper.IsRequestRaised);
            Assert.IsTrue(info.IsPathValid);
        }

        [Test]
        public void InvalidModelStorePathRaisesErrorNotification()
        {
            var info = _uut.ValidateModelStoreInformation(InvalidModelStorePath);
            Assert.IsTrue(_notificationHelper.IsRequestRaised);
            Assert.IsFalse(info.IsPathValid);
        }

        [Test]
        public void InvalidModelStorePathErrorHasCorrectMessage()
        {
            _uut.ValidateModelStoreInformation(InvalidModelStorePath);

            var actual = _notificationHelper.Context;
            var expected = new Notification
            {
                Caption = Properties.SessionManager.Options_Title,
                Message = Properties.SessionManager.OptionPageInvalidModelStorePathMessage
            };

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void UsageModelsTableContentIsGeneratedByMergingStrategy()
        {
            CollectionAssert.AreEquivalent(_usageModelsTableContent, _uut.UsageModelsTableContent);
        }

        [Test]
        public void InstallModelTriggersLoad()
        {
            var row = Mock.Of<IUsageModelsTableRow>();
            _uut.InstallModel.Execute(row);
            Thread.Sleep(100);
            Mock.Get(row).Verify(r => r.LoadModel(), Times.Once);
        }

        [Test]
        public void UpdateModelTriggersLoad()
        {
            var row = Mock.Of<IUsageModelsTableRow>();
            _uut.UpdateModel.Execute(row);
            Thread.Sleep(100);
            Mock.Get(row).Verify(r => r.LoadModel(), Times.Once);
        }

        [Test]
        public void RemoveModelTriggersRemove()
        {
            var row = Mock.Of<IUsageModelsTableRow>();
            _uut.RemoveModel.Execute(row);
            Thread.Sleep(100);
            Mock.Get(row).Verify(r => r.RemoveModel(), Times.Once);
        }

        [Test]
        public void InstallModelCanExecuteChecksIsInstallable()
        {
            var row = Mock.Of<IUsageModelsTableRow>();
            _uut.InstallModel.CanExecute(row);
            Mock.Get(row).Verify(r => r.IsInstallable);
        }

        [Test]
        public void UpdateModelCanExecuteChecksIsUpdateable()
        {
            var row = Mock.Of<IUsageModelsTableRow>();
            _uut.UpdateModel.CanExecute(row);
            Mock.Get(row).Verify(r => r.IsUpdateable);
        }

        [Test]
        public void RemoveModelCanExecuteChecksIsRemoveable()
        {
            var row = Mock.Of<IUsageModelsTableRow>();
            _uut.RemoveModel.CanExecute(row);
            Mock.Get(row).Verify(r => r.IsRemoveable);
        }

        [Test]
        public void InstallModelCannotExecuteForNull()
        {
            Assert.IsFalse(_uut.InstallModel.CanExecute(null));
        }

        [Test]
        public void UpdateModelCannotExecuteForNull()
        {
            Assert.IsFalse(_uut.UpdateModel.CanExecute(null));
        }

        [Test]
        public void RemoveModelCannotExecuteForNull()
        {
            Assert.IsFalse(_uut.RemoveModel.CanExecute(null));
        }

        [Test]
        public void InstallAllModelsTriggersLoadOnAllRows()
        {
            _uut.InstallAllModelsCommand.Execute(null);
            Thread.Sleep(100);

            foreach (var row in _uut.UsageModelsTableContent)
            {
                Mock.Get(row).Verify(r => r.IsInstallable);
                Mock.Get(row).Verify(r => r.LoadModel());
            }
        }

        [Test]
        public void UpdateAllModelsTriggersLoadOnAllRows()
        {
            _uut.UpdateAllModelsCommand.Execute(null);
            Thread.Sleep(100);

            foreach (var row in _uut.UsageModelsTableContent)
            {
                Mock.Get(row).Verify(r => r.IsUpdateable);
                Mock.Get(row).Verify(r => r.LoadModel());
            }
        }

        [Test]
        public void RemoveAllModelsTriggersRemoveOnAllRows()
        {
            _uut.RemoveAllModelsCommand.Execute(null);
            Thread.Sleep(100);

            foreach (var row in _uut.UsageModelsTableContent)
            {
                Mock.Get(row).Verify(r => r.IsRemoveable);
                Mock.Get(row).Verify(r => r.RemoveModel());
            }
        }
    }
}
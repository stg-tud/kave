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
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.CodeCompletion;
using KaVE.VS.FeedbackGenerator.UserControls.OptionPage.UsageModelOptions;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.OptionPage.UsageModelOptions
{
    public class UsageModelOptionsViewModelTest
    {
        private UsageModelOptionsViewModel _uut;
        private IEnumerable<IUsageModelsTableRow> _usageModelsTableContent;
        private IPBNProposalItemsProvider _proposalItemsProvider;
        private IUsageModelMergingStrategy _mergingStrategy;
        private ILocalPBNRecommenderStore _localStore;
        private IRemotePBNRecommenderStore _remoteStore;

        [SetUp]
        public void SetUp()
        {
            _usageModelsTableContent = new List<IUsageModelsTableRow>
            {
                GenerateRowMock(),
                GenerateRowMock(),
                GenerateRowMock()
            };

            _proposalItemsProvider = Mock.Of<IPBNProposalItemsProvider>();
            _localStore = Mock.Of<ILocalPBNRecommenderStore>();
            _remoteStore = Mock.Of<IRemotePBNRecommenderStore>();
            Registry.RegisterComponent(_proposalItemsProvider);
            Registry.RegisterComponent(_localStore);
            Registry.RegisterComponent(_remoteStore);

            _mergingStrategy = Mock.Of<IUsageModelMergingStrategy>();
            Mock.Get(_mergingStrategy)
                .Setup(
                    strategy =>
                        strategy.MergeAvailableModels(
                            _localStore,
                            _remoteStore))
                .Returns(() => _usageModelsTableContent);

            _uut = new UsageModelOptionsViewModel(
                _mergingStrategy,
                new KaVEBackgroundWorker());
        }

        [TearDown]
        public void ClearRegistry()
        {
            Registry.Clear();
        }

        [Test]
        public void UsageModelsTableContentIsGeneratedByMergingStrategy()
        {
            Assert.AreSame(_usageModelsTableContent, _uut.UsageModelsTableContent);
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

        [Test]
        public void ShouldClearCachedModelsOnReload()
        {
            _uut.ReloadModelsCommand.Execute(null);
            Mock.Get(_proposalItemsProvider).Verify(provider => provider.Clear(), Times.Once);
        }

        [Test]
        public void ShouldReloadModelsTableOnReload()
        {
            _usageModelsTableContent = new List<IUsageModelsTableRow> {GenerateRowMock()};
            _uut.ReloadModelsCommand.Execute(null);
            Assert.AreSame(_usageModelsTableContent, _uut.UsageModelsTableContent);
        }

        private static IUsageModelsTableRow GenerateRowMock()
        {
            var rowMock = Mock.Of<IUsageModelsTableRow>();
            Mock.Get(rowMock).Setup(row => row.IsInstallable).Returns(true);
            Mock.Get(rowMock).Setup(row => row.IsUpdateable).Returns(true);
            Mock.Get(rowMock).Setup(row => row.IsRemoveable).Returns(true);
            return rowMock;
        }
    }
}
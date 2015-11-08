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
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.CodeCompletion;
using KaVE.Commons.Utils.CodeCompletion.Impl;
using KaVE.Commons.Utils.CodeCompletion.Stores;
using KaVE.Commons.Utils.Collections;
using KaVE.RS.Commons.Utils;
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

        private static IEnumerable<UsageModelDescriptor> SomeLocalModels
        {
            get
            {
                return new KaVEList<UsageModelDescriptor>
                {
                    new UsageModelDescriptor(new CoReTypeName("LSomeAssembly/SomeType"), 2),
                    new UsageModelDescriptor(new CoReTypeName("LSomeOtherAssembly/OnlyLocalType"), 3)
                };
            }
        }

        private static IEnumerable<UsageModelDescriptor> SomeRemoteModels
        {
            get
            {
                return new KaVEList<UsageModelDescriptor>
                {
                    new UsageModelDescriptor(new CoReTypeName("LSomeAssembly/SomeType"), 4),
                    new UsageModelDescriptor(new CoReTypeName("LSomeAssembly/OnlyRemoteType"), 5)
                };
            }
        }

        private static IEnumerable<UsageModelsTableRow> ExpectedUsageModelsTableContent
        {
            get
            {
                return new KaVEList<UsageModelsTableRow>
                {
                    new UsageModelsTableRow(
                        Mock.Of<ILocalPBNRecommenderStore>(),
                        Mock.Of<IRemotePBNRecommenderStore>(),
                        new CoReTypeName("LSomeAssembly/SomeType"),
                        2,
                        4),
                    new UsageModelsTableRow(
                        Mock.Of<ILocalPBNRecommenderStore>(),
                        Mock.Of<IRemotePBNRecommenderStore>(),
                        new CoReTypeName("LSomeOtherAssembly/OnlyLocalType"),
                        3,
                        null),
                    new UsageModelsTableRow(
                        Mock.Of<ILocalPBNRecommenderStore>(),
                        Mock.Of<IRemotePBNRecommenderStore>(),
                        new CoReTypeName("LSomeAssembly/OnlyRemoteType"),
                        null,
                        5)
                };
            }
        }

        private UsageModelOptionsViewModel _uut;
        private InteractionRequestTestHelper<Notification> _notificationHelper;
        private ILocalPBNRecommenderStore _localStore;
        private IRemotePBNRecommenderStore _remoteStore;

        [SetUp]
        public void SetUp()
        {
            var mergingStrategy = Mock.Of<IUsageModelMergingStrategy>();
            Mock.Get(mergingStrategy).Setup(strategy => strategy.MergeAvailableModels(_localStore, _remoteStore)).Returns(ExpectedUsageModelsTableContent);

            _uut = new UsageModelOptionsViewModel(mergingStrategy, new KaVEBackgroundWorker());
            _notificationHelper = _uut.ErrorNotificationRequest.NewTestHelper();

            _localStore = Mock.Of<ILocalPBNRecommenderStore>();
            Mock.Get(_localStore).Setup(store => store.GetAvailableModels()).Returns(SomeLocalModels);
            Registry.RegisterComponent(_localStore);

            _remoteStore = Mock.Of<IRemotePBNRecommenderStore>();
            Mock.Get(_remoteStore).Setup(store => store.GetAvailableModels()).Returns(SomeRemoteModels);
            Registry.RegisterComponent(_remoteStore);
        }

        [TearDown]
        public void ClearRegistry()
        {
            Registry.Clear();
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
        public void GeneratingUsageModelsTableContent()
        {
            // TODO test merging logic in UsageModelMergingStrategy
            CollectionAssert.AreEquivalent(ExpectedUsageModelsTableContent, _uut.UsageModelsTableContent);
        }
    }
}
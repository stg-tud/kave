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

using System;
using JetBrains.Application.DataContext;
using JetBrains.DataFlow;
using JetBrains.UI.Options;
using KaVE.RS.Commons;
using KaVE.RS.Commons.Settings;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.OptionPage
{
    internal class BaseOptionPageUserControlTest : BaseUserControlTest
    {
        protected readonly Lifetime TestLifetime = EternalLifetime.Instance;
        protected Mock<ISettingsStore> MockSettingsStore;
        protected Mock<IMessageBoxCreator> MockMessageBoxCreator;
        protected Mock<IActionExecutor> MockActionExecutor;
        protected DataContexts TestDataContexts;
        protected OptionsSettingsSmartContext TestOptionsSettingsSmartContext;

        [SetUp]
        public void SetUp()
        {
            MockMessageBoxCreator = new Mock<IMessageBoxCreator>();
            MockActionExecutor = new Mock<IActionExecutor>();

            MockSettingsStore = new Mock<ISettingsStore>();

            TestDataContexts = new DataContexts(
                TestLifetime,
                new FindDataRules(TestLifetime, new Mock<IViewable<IFindDataRules>>().Object));

            TestOptionsSettingsSmartContext = null;
        }

        protected void VerifyActionExecuted(Func<Times> times)
        {
            MockActionExecutor.Verify(
                actionExecutor => actionExecutor.ExecuteActionGuarded<SettingsCleaner>(It.IsAny<IDataContext>()),
                times);
        }

        protected void SetConfirmationAnswerTo(bool answer)
        {
            MockMessageBoxCreator.Setup(messageBoxCreator => messageBoxCreator.ShowYesNo(It.IsAny<string>()))
                                 .Returns(answer);
        }
    }
}
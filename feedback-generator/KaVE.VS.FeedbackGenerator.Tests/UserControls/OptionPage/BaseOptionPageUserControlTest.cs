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
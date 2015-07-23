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

using KaVE.VS.FeedbackGenerator.Interactivity;
using KaVE.VS.FeedbackGenerator.Tests.Interactivity;
using KaVE.VS.FeedbackGenerator.UserControls.OptionPage.UsageModelOptions;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.OptionPage.UsageModelOptions
{
    public class UsageModelOptionsViewModelTest
    {
        private const string TestModelStorePath = @"c:/";
        private const string InvalidModelStorePath = @"c:/some/folder/that/surely/does/not/exist";

        private UsageModelOptionsViewModel _uut;
        private InteractionRequestTestHelper<Notification> _notificationHelper;

        [SetUp]
        public void SetUp()
        {
            _uut = new UsageModelOptionsViewModel();
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
    }
}
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

using KaVE.VsFeedbackGenerator.Interactivity;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using KaVE.VsFeedbackGenerator.Tests.Interactivity;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.Presentation
{
    [TestFixture]
    internal class OptionPageViewModelTest
    {
        private const string TestUploadUrl = "http://foo.bar/";
        private const string InvalidUploadUrl = "ht8tp://foo.bar/";

        private const string TestUploadPrefix = "http://pre";
        private const string InvalidUploadPrefix = "ht5tp://";

        private OptionPageViewModel _uut;
        private InteractionRequestTestHelper<Notification> _notificationHelper;

        [SetUp]
        public void SetUp()
        {
            _uut = new OptionPageViewModel();
            _notificationHelper = _uut.ErrorNotificationRequest.NewTestHelper();
        }

        [Test]
        public void InvalidUploadUrlRaisesErrorNotification()
        {
            var info = _uut.ValidateUploadInformation(InvalidUploadUrl, TestUploadPrefix);
            Assert.IsTrue(_notificationHelper.IsRequestRaised);
            Assert.IsFalse(info.IsUrlValid);
        }

        [Test]
        public void InvalidUploadPrefixRaisesErrorNotification()
        {
            var info = _uut.ValidateUploadInformation(TestUploadUrl, InvalidUploadPrefix);
            Assert.IsTrue(_notificationHelper.IsRequestRaised);
            Assert.IsFalse(info.IsPrefixValid);
        }

        [Test]
        public void ValidUploadInformationRaisesNoErrorNotification()
        {
            var info = _uut.ValidateUploadInformation(TestUploadUrl, TestUploadPrefix);
            Assert.IsFalse(_notificationHelper.IsRequestRaised);
            Assert.IsTrue(info.IsUrlValid);
            Assert.IsTrue(info.IsPrefixValid);
            Assert.IsTrue(info.IsValidUploadInformation);
        }

        [Test]
        public void InvalidUploadPrefixErrorHasCorrectMessage()
        {
            _uut.ValidateUploadInformation(TestUploadUrl, InvalidUploadPrefix);

            var actual = _notificationHelper.Context;
            var expected = new Notification
            {
                Caption = Properties.SessionManager.Options_Title,
                Message = Properties.SessionManager.OptionPageErrorMessage,
            };

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void InvalidUploadUrlErrorHasCorrectMessage()
        {
            _uut.ValidateUploadInformation(InvalidUploadUrl, TestUploadPrefix);

            var actual = _notificationHelper.Context;
            var expected = new Notification
            {
                Caption = Properties.SessionManager.Options_Title,
                Message = Properties.SessionManager.OptionPageErrorMessage,
            };

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void InvalidUrlAndPrefixRaisesOneNotification()
        {
            _uut.ValidateUploadInformation(InvalidUploadPrefix, InvalidUploadPrefix);
            Assert.AreEqual(1, _notificationHelper.NumberOfRequests);
        }
    }
}
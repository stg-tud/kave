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
using KaVE.Commons.Utils.DateTime;
using KaVE.VS.FeedbackGenerator.Settings;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Settings
{
    internal class UploadSettingsTest
    {
        [Test]
        public void ShouldBeInvalidWithoutData()
        {
            var uut = new UploadSettings();

            Assert.IsFalse(uut.IsInitialized());
        }

        [Test]
        public void ShouldBeInvalidWithoutLastNotificationTime()
        {
            var uut = new UploadSettings {LastUploadDate = DateTime.Now};

            Assert.IsFalse(uut.IsInitialized());
        }

        [Test]
        public void ShouldBeInvalidWithoutLastUploadTime()
        {
            var uut = new UploadSettings {LastNotificationDate = DateTime.Now};

            Assert.IsFalse(uut.IsInitialized());
        }

        [Test]
        public void ShouldBeValidWithAllData()
        {
            var uut = new UploadSettings {LastNotificationDate = DateTime.Now, LastUploadDate = DateTime.Now};

            Assert.IsTrue(uut.IsInitialized());
        }

        [Test]
        public void ShouldInitializeWithCurrentDate()
        {
            var uut = new UploadSettings();

            uut.Initialize();
            var dateTime = DateTime.Now;

            Assert.IsTrue(uut.IsInitialized());
            var comparer = new SimilarDateTimeComparer(50);
            Assert.IsTrue(comparer.Equal(dateTime, uut.LastNotificationDate));
            Assert.IsTrue(comparer.Equal(dateTime, uut.LastUploadDate));
        }
    }
}
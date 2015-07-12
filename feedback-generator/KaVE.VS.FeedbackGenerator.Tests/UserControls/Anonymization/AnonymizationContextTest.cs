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
using KaVE.VS.FeedbackGenerator.Settings.ExportSettingsSuite;
using KaVE.VS.FeedbackGenerator.UserControls.Anonymization;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.Anonymization
{
    internal class AnonymizationContextTest
    {
        private ExportSettings _exportSettings;
        private List<string> _updatedProperties;

        private AnonymizationContext _sut;

        [SetUp]
        public void SetUp()
        {
            _updatedProperties = new List<string>();
            _exportSettings = new ExportSettings
            {
                RemoveCodeNames = false,
                RemoveSessionIDs = false,
                RemoveDurations = false,
                RemoveStartTimes = false
            };
            _sut = new AnonymizationContext(_exportSettings);
            _sut.PropertyChanged += (sender, args) => { _updatedProperties.Add(args.PropertyName); };
        }

        [Test]
        public void RemoveCodeNames_PropagationFromCode()
        {
            _exportSettings.RemoveCodeNames = false;
            Assert.False(_sut.RemoveCodeNames);
            _exportSettings.RemoveCodeNames = true;
            Assert.True(_sut.RemoveCodeNames);
        }

        [Test]
        public void RemoveCodeNames_PropagationToCode()
        {
            _sut.RemoveCodeNames = false;
            Assert.False(_exportSettings.RemoveCodeNames);
            _sut.RemoveCodeNames = true;
            Assert.True(_exportSettings.RemoveCodeNames);
        }

        [Test]
        public void RemoveCodeNames_PropertyChange()
        {
            _sut.RemoveCodeNames = true;
            AssertNotifications("RemoveCodeNames");
        }

        [Test]
        public void RemoveSessionIDs_PropagationFromCode()
        {
            _exportSettings.RemoveSessionIDs = false;
            Assert.False(_sut.RemoveSessionIDs);
            _exportSettings.RemoveSessionIDs = true;
            Assert.True(_sut.RemoveSessionIDs);
        }

        [Test]
        public void RemoveSessionIDs_PropagationToCode()
        {
            _sut.RemoveSessionIDs = false;
            Assert.False(_exportSettings.RemoveSessionIDs);
            _sut.RemoveSessionIDs = true;
            Assert.True(_exportSettings.RemoveSessionIDs);
        }

        [Test]
        public void RemoveSessionIDs_PropertyChange()
        {
            _sut.RemoveSessionIDs = true;
            AssertNotifications("RemoveSessionIDs");
        }

        [Test]
        public void RemoveDurations_PropagationFromCode()
        {
            _exportSettings.RemoveDurations = false;
            Assert.False(_sut.RemoveDurations);
            _exportSettings.RemoveDurations = true;
            Assert.True(_sut.RemoveDurations);
        }

        [Test]
        public void RemoveDurationss_PropagationToCode()
        {
            _sut.RemoveDurations = false;
            Assert.False(_exportSettings.RemoveDurations);
            _sut.RemoveDurations = true;
            Assert.True(_exportSettings.RemoveDurations);
        }

        [Test]
        public void RemoveDurations_PropertyChange()
        {
            _sut.RemoveDurations = true;
            AssertNotifications("RemoveDurations");
        }

        [Test]
        public void RemoveStartTimes_PropagationFromCode()
        {
            _exportSettings.RemoveStartTimes = false;
            Assert.False(_sut.RemoveStartTimes);
            _exportSettings.RemoveStartTimes = true;
            Assert.True(_sut.RemoveStartTimes);
        }

        [Test]
        public void RemoveStartTimes_PropagationToCode()
        {
            _sut.RemoveStartTimes = false;
            Assert.False(_exportSettings.RemoveStartTimes);
            _sut.RemoveStartTimes = true;
            Assert.True(_exportSettings.RemoveStartTimes);
        }

        [Test]
        public void RemoveStartTimes_PropertyChange()
        {
            _sut.RemoveStartTimes = true;
            AssertNotifications("RemoveStartTimes");
        }

        private void AssertNotifications(params string[] expecteds)
        {
            CollectionAssert.AreEqual(expecteds, _updatedProperties);
        }
    }
}
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
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.UserControls.Anonymization;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.Anonymization
{
    internal class AnonymizationContextTest
    {
        private AnonymizationSettings _anonymizationSettings;
        private List<string> _updatedProperties;

        private AnonymizationContext _sut;

        [SetUp]
        public void SetUp()
        {
            _updatedProperties = new List<string>();
            _anonymizationSettings = new AnonymizationSettings
            {
                RemoveCodeNames = false,
                RemoveSessionIDs = false,
                RemoveDurations = false,
                RemoveStartTimes = false
            };
            _sut = new AnonymizationContext(_anonymizationSettings);
            _sut.PropertyChanged += (sender, args) => { _updatedProperties.Add(args.PropertyName); };
        }

        [Test]
        public void RemoveCodeNames_PropagationFromCode()
        {
            _anonymizationSettings.RemoveCodeNames = false;
            Assert.False(_sut.RemoveCodeNames);
            _anonymizationSettings.RemoveCodeNames = true;
            Assert.True(_sut.RemoveCodeNames);
        }

        [Test]
        public void RemoveCodeNames_PropagationToCode()
        {
            _sut.RemoveCodeNames = false;
            Assert.False(_anonymizationSettings.RemoveCodeNames);
            _sut.RemoveCodeNames = true;
            Assert.True(_anonymizationSettings.RemoveCodeNames);
        }

        [Test]
        public void RemoveCodeNames_PropertyChange()
        {
            _sut.RemoveCodeNames = true;
            AssertNotifications("RemoveCodeNames");
        }

        [Test]
        public void RemoveDurations_PropagationFromCode()
        {
            _anonymizationSettings.RemoveDurations = false;
            Assert.False(_sut.RemoveDurations);
            _anonymizationSettings.RemoveDurations = true;
            Assert.True(_sut.RemoveDurations);
        }

        [Test]
        public void RemoveDurationss_PropagationToCode()
        {
            _sut.RemoveDurations = false;
            Assert.False(_anonymizationSettings.RemoveDurations);
            _sut.RemoveDurations = true;
            Assert.True(_anonymizationSettings.RemoveDurations);
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
            _anonymizationSettings.RemoveStartTimes = false;
            Assert.False(_sut.RemoveStartTimes);
            _anonymizationSettings.RemoveStartTimes = true;
            Assert.True(_sut.RemoveStartTimes);
        }

        [Test]
        public void RemoveStartTimes_PropagationToCode()
        {
            _sut.RemoveStartTimes = false;
            Assert.False(_anonymizationSettings.RemoveStartTimes);
            _sut.RemoveStartTimes = true;
            Assert.True(_anonymizationSettings.RemoveStartTimes);
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
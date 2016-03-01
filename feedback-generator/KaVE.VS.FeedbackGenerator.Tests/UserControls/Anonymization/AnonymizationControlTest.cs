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

using KaVE.Commons.TestUtils.UserControls;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.UserControls.Anonymization;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.Anonymization
{
    [RequiresSTA]
    internal class AnonymizationControlTest : BaseUserControlTest
    {
        private AnonymizationContext _context;
        private AnonymizationSettings _anonymizationSettings;

        [SetUp]
        public void SetUp()
        {
            _anonymizationSettings = new AnonymizationSettings
            {
                RemoveStartTimes = false,
                RemoveDurations = false,
                RemoveCodeNames = false
            };
            _context = new AnonymizationContext(_anonymizationSettings);
        }

        public AnonymizationControl OpenWindow()
        {
            return OpenWindow(new AnonymizationControl {DataContext = _context});
        }

        [Test]
        public void CheckboxesReflectContextDeactivated()
        {
            var sut = OpenWindow();
            UserControlAssert.IsNotChecked(sut.RemoveStartTimesCheckBox);
            UserControlAssert.IsNotChecked(sut.RemoveDurationsCheckBox);
            UserControlAssert.IsNotChecked(sut.RemoveCodeNamesCheckBox);
        }

        [Test]
        public void CheckboxesReflectContext_Activated()
        {
            _anonymizationSettings.RemoveStartTimes = true;
            _anonymizationSettings.RemoveDurations = true;
            _anonymizationSettings.RemoveCodeNames = true;
            var sut = OpenWindow();
            UserControlAssert.IsChecked(sut.RemoveStartTimesCheckBox);
            UserControlAssert.IsChecked(sut.RemoveDurationsCheckBox);
            UserControlAssert.IsChecked(sut.RemoveCodeNamesCheckBox);
        }

        [Test]
        public void CheckboxesFromCode_RemoveStartTimes()
        {
            var sut = OpenWindow();
            _context.RemoveStartTimes = true;
            UserControlAssert.IsChecked(sut.RemoveStartTimesCheckBox);
            UserControlAssert.IsNotChecked(sut.RemoveDurationsCheckBox);
            UserControlAssert.IsNotChecked(sut.RemoveCodeNamesCheckBox);
        }

        [Test]
        public void CheckboxesFromCode_RemoveDurations()
        {
            var sut = OpenWindow();
            _context.RemoveDurations = true;
            UserControlAssert.IsNotChecked(sut.RemoveStartTimesCheckBox);
            UserControlAssert.IsChecked(sut.RemoveDurationsCheckBox);
            UserControlAssert.IsNotChecked(sut.RemoveCodeNamesCheckBox);
        }

        [Test]
        public void CheckboxesFromCode_RemoveCodeNames()
        {
            var sut = OpenWindow();
            _context.RemoveCodeNames = true;
            UserControlAssert.IsNotChecked(sut.RemoveStartTimesCheckBox);
            UserControlAssert.IsNotChecked(sut.RemoveDurationsCheckBox);
            UserControlAssert.IsChecked(sut.RemoveCodeNamesCheckBox);
        }

        [Test]
        public void CheckboxesToCode_RemoveStartTimes()
        {
            var sut = OpenWindow();
            sut.RemoveStartTimesCheckBox.Toggle();
            Assert.True(_anonymizationSettings.RemoveStartTimes);
            Assert.False(_anonymizationSettings.RemoveDurations);
            Assert.False(_anonymizationSettings.RemoveSessionIDs);
            Assert.False(_anonymizationSettings.RemoveCodeNames);
        }

        [Test]
        public void CheckboxesToCode_RemoveDurations()
        {
            var sut = OpenWindow();
            sut.RemoveDurationsCheckBox.Toggle();
            Assert.False(_anonymizationSettings.RemoveStartTimes);
            Assert.True(_anonymizationSettings.RemoveDurations);
            Assert.False(_anonymizationSettings.RemoveSessionIDs);
            Assert.False(_anonymizationSettings.RemoveCodeNames);
        }

        [Test]
        public void CheckboxesToCode_RemoveCodeNames()
        {
            var sut = OpenWindow();
            sut.RemoveCodeNamesCheckBox.Toggle();
            Assert.False(_anonymizationSettings.RemoveStartTimes);
            Assert.False(_anonymizationSettings.RemoveDurations);
            Assert.False(_anonymizationSettings.RemoveSessionIDs);
            Assert.True(_anonymizationSettings.RemoveCodeNames);
        }
    }
}
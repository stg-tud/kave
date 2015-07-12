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
using KaVE.VS.FeedbackGenerator.Settings.ExportSettingsSuite;
using KaVE.VS.FeedbackGenerator.UserControls.Anonymization;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.Anonymization
{
    [RequiresSTA]
    internal class AnonymizationControlTest : BaseUserControlTest
    {
        private AnonymizationContext _context;
        private ExportSettings _exportSettings;

        [SetUp]
        public void SetUp()
        {
            _exportSettings = new ExportSettings
            {
                RemoveStartTimes = false,
                RemoveDurations = false,
                RemoveSessionIDs = false,
                RemoveCodeNames = false
            };
            _context = new AnonymizationContext(_exportSettings);
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
            UserControlAssert.IsNotChecked(sut.RemoveSessionIDsCheckBox);
            UserControlAssert.IsNotChecked(sut.RemoveCodeNamesCheckBox);
        }

        [Test]
        public void CheckboxesReflectContext_Activated()
        {
            _exportSettings.RemoveStartTimes = true;
            _exportSettings.RemoveDurations = true;
            _exportSettings.RemoveSessionIDs = true;
            _exportSettings.RemoveCodeNames = true;
            var sut = OpenWindow();
            UserControlAssert.IsChecked(sut.RemoveStartTimesCheckBox);
            UserControlAssert.IsChecked(sut.RemoveDurationsCheckBox);
            UserControlAssert.IsChecked(sut.RemoveSessionIDsCheckBox);
            UserControlAssert.IsChecked(sut.RemoveCodeNamesCheckBox);
        }

        [Test]
        public void CheckboxesFromCode_RemoveStartTimes()
        {
            var sut = OpenWindow();
            _context.RemoveStartTimes = true;
            UserControlAssert.IsChecked(sut.RemoveStartTimesCheckBox);
            UserControlAssert.IsNotChecked(sut.RemoveDurationsCheckBox);
            UserControlAssert.IsNotChecked(sut.RemoveSessionIDsCheckBox);
            UserControlAssert.IsNotChecked(sut.RemoveCodeNamesCheckBox);
        }

        [Test]
        public void CheckboxesFromCode_RemoveDurations()
        {
            var sut = OpenWindow();
            _context.RemoveDurations = true;
            UserControlAssert.IsNotChecked(sut.RemoveStartTimesCheckBox);
            UserControlAssert.IsChecked(sut.RemoveDurationsCheckBox);
            UserControlAssert.IsNotChecked(sut.RemoveSessionIDsCheckBox);
            UserControlAssert.IsNotChecked(sut.RemoveCodeNamesCheckBox);
        }

        [Test]
        public void CheckboxesFromCode_RemoveSessionIDs()
        {
            var sut = OpenWindow();
            _context.RemoveSessionIDs = true;
            UserControlAssert.IsNotChecked(sut.RemoveStartTimesCheckBox);
            UserControlAssert.IsNotChecked(sut.RemoveDurationsCheckBox);
            UserControlAssert.IsChecked(sut.RemoveSessionIDsCheckBox);
            UserControlAssert.IsNotChecked(sut.RemoveCodeNamesCheckBox);
        }

        [Test]
        public void CheckboxesFromCode_RemoveCodeNames()
        {
            var sut = OpenWindow();
            _context.RemoveCodeNames = true;
            UserControlAssert.IsNotChecked(sut.RemoveStartTimesCheckBox);
            UserControlAssert.IsNotChecked(sut.RemoveDurationsCheckBox);
            UserControlAssert.IsNotChecked(sut.RemoveSessionIDsCheckBox);
            UserControlAssert.IsChecked(sut.RemoveCodeNamesCheckBox);
        }

        [Test]
        public void CheckboxesToCode_RemoveStartTimes()
        {
            var sut = OpenWindow();
            sut.RemoveStartTimesCheckBox.Toggle();
            Assert.True(_exportSettings.RemoveStartTimes);
            Assert.False(_exportSettings.RemoveDurations);
            Assert.False(_exportSettings.RemoveSessionIDs);
            Assert.False(_exportSettings.RemoveCodeNames);
        }

        [Test]
        public void CheckboxesToCode_RemoveDurations()
        {
            var sut = OpenWindow();
            sut.RemoveDurationsCheckBox.Toggle();
            Assert.False(_exportSettings.RemoveStartTimes);
            Assert.True(_exportSettings.RemoveDurations);
            Assert.False(_exportSettings.RemoveSessionIDs);
            Assert.False(_exportSettings.RemoveCodeNames);
        }

        [Test]
        public void CheckboxesToCode_RemoveSessionIDs()
        {
            var sut = OpenWindow();
            sut.RemoveSessionIDsCheckBox.Toggle();
            Assert.False(_exportSettings.RemoveStartTimes);
            Assert.False(_exportSettings.RemoveDurations);
            Assert.True(_exportSettings.RemoveSessionIDs);
            Assert.False(_exportSettings.RemoveCodeNames);
        }

        [Test]
        public void CheckboxesToCode_RemoveCodeNames()
        {
            var sut = OpenWindow();
            sut.RemoveCodeNamesCheckBox.Toggle();
            Assert.False(_exportSettings.RemoveStartTimes);
            Assert.False(_exportSettings.RemoveDurations);
            Assert.False(_exportSettings.RemoveSessionIDs);
            Assert.True(_exportSettings.RemoveCodeNames);
        }
    }
}
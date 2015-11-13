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

using System.Globalization;
using System.IO;
using KaVE.VS.FeedbackGenerator.UserControls.ValidationRules;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.ValidationRules
{
    internal class UsageModelsUriValidationRuleTest
    {
        private UsageModelsUriValidationRule _uut;

        private string _validPath;
        private string _directoryWithoutIndexFile;

        private string IndexFile
        {
            get { return Path.Combine(_validPath, "index.json.gz"); }
        }

        [SetUp]
        public void Setup()
        {
            _uut = new UsageModelsUriValidationRule();

            _validPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_validPath);
            File.Create(IndexFile).Close();

            _directoryWithoutIndexFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_directoryWithoutIndexFile);
        }

        [TearDown]
        public void DeleteTmpFolders()
        {
            if (Directory.Exists(_validPath))
            {
                Directory.Delete(_validPath, true);
            }
            if (Directory.Exists(_directoryWithoutIndexFile))
            {
                Directory.Delete(_directoryWithoutIndexFile, true);
            }
        }

        #region file path validation tests

        [Test]
        public void ExistingFolderWithIndexFileShouldBeValid()
        {
            Assert.IsTrue(_uut.Validate(_validPath, CultureInfo.InvariantCulture).IsValid);
        }

        [Test]
        public void ExistingFolderWithoutIndexFileShouldNotBeValid()
        {
            Assert.IsFalse(_uut.Validate(_directoryWithoutIndexFile, CultureInfo.InvariantCulture).IsValid);
        }

        [Test]
        public void NonExistingFolderShouldNotBeValid()
        {
            Assert.IsFalse(_uut.Validate(@"C:\does\not\exist\", CultureInfo.InvariantCulture).IsValid);
        }

        #endregion

        #region url validation tests

        // TODO validate index file exists for urls

        [Test]
        public void CorrectHttpUriShouldBeValid()
        {
            Assert.IsTrue(_uut.Validate("http://www.kave.cc/", CultureInfo.InvariantCulture).IsValid);
        }

        [Test]
        public void IncorrectHttpUriShouldBeInvalid()
        {
            Assert.IsTrue(_uut.Validate("http://invalidurl", CultureInfo.InvariantCulture).IsValid);
        }

        #endregion
    }
}
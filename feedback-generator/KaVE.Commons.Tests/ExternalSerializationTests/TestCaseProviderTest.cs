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
using System.IO;
using System.Linq;
using KaVE.Commons.Model.Events.CompletionEvents;
using NUnit.Framework;

namespace KaVE.Commons.Tests.ExternalSerializationTests
{
    internal class TestCaseProviderTest
    {
        private string _baseDirectory;

        private const string ExpectedFirstName = @"TestSuite\TestCases\FirstTest";
        private const string ExpectedFirstInput = "firstInputContent";
        private const string ExpectedCompact = "expectedCompactContent";
        private const string ExpectedFormatted = "expectedFormattedContent";
        private static readonly Type ExpectedSerializedType = typeof (CompletionEvent);
        private static string _testCasesDirectory;

        [SetUp]
        public void Setup()
        {
            _baseDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            GenerateTestCaseStructure(_baseDirectory);
        }

        [TearDown]
        public void Teardown()
        {
            if (Directory.Exists(_baseDirectory))
            {
                Directory.Delete(_baseDirectory, true);
            }
        }

        [Test]
        public void ShouldFindTestCasesRecursively()
        {
            var testCases = TestCaseProvider.GetTestCases(_baseDirectory);
            Assert.AreEqual(2, testCases.Count());
        }

        [Test]
        public void ShouldFindInput()
        {
            var firstTestCase = TestCaseProvider.GetTestCases(_baseDirectory).First();
            Assert.AreEqual(ExpectedFirstInput, firstTestCase.Input);
        }

        [Test]
        public void ShouldFindCompactExpected()
        {
            var firstTestCase = TestCaseProvider.GetTestCases(_baseDirectory).First();
            Assert.AreEqual(ExpectedCompact, firstTestCase.ExpectedCompact);
        }

        [Test]
        public void ShouldFindFormattedExpected()
        {
            var firstTestCase = TestCaseProvider.GetTestCases(_baseDirectory).First();
            Assert.AreEqual(ExpectedFormatted, firstTestCase.ExpectedFormatted);
        }

        [Test]
        public void ShouldFindName()
        {
            var firstTestCase = TestCaseProvider.GetTestCases(_baseDirectory).First();
            Assert.AreEqual(ExpectedFirstName, firstTestCase.Name);
        }

        [Test]
        public void ShouldGetSerializedTypeFromTypeHint()
        {
            var firstTestCase = TestCaseProvider.GetTestCases(_baseDirectory).First();
            Assert.AreEqual(ExpectedSerializedType, firstTestCase.SerializedType);
        }

        [Test]
        public void SettingsFileShouldBeOptional()
        {
            File.Delete(Path.Combine(_testCasesDirectory, "settings.ini"));
            var firstTestCase = TestCaseProvider.GetTestCases(_baseDirectory).First();
            Assert.AreEqual(typeof (object), firstTestCase.SerializedType);
        }

        [Test]
        public void ExpectedFormattedFileShouldBeOptional()
        {
            File.Delete(Path.Combine(_testCasesDirectory, "expected-formatted.json"));
            var firstTestCase = TestCaseProvider.GetTestCases(_baseDirectory).First();
            Assert.IsNull(firstTestCase.ExpectedFormatted);
        }

        private static void GenerateTestCaseStructure(string baseDirectory)
        {
            Directory.CreateDirectory(baseDirectory);

            var suiteBasePath = Path.Combine(baseDirectory, "TestSuite");
            Directory.CreateDirectory(suiteBasePath);

            _testCasesDirectory = Path.Combine(suiteBasePath, "TestCases");
            Directory.CreateDirectory(_testCasesDirectory);

            var firstInputFile = Path.Combine(_testCasesDirectory, "FirstTest.json");
            File.WriteAllText(firstInputFile, ExpectedFirstInput);

            var secondInputFile = Path.Combine(_testCasesDirectory, "SecondTest.json");
            File.WriteAllText(secondInputFile, "secondInputContent");

            var expectedCompactFile = Path.Combine(_testCasesDirectory, "expected-compact.json");
            File.WriteAllText(expectedCompactFile, ExpectedCompact);

            var expectedPrettyPrintFile = Path.Combine(_testCasesDirectory, "expected-formatted.json");
            File.WriteAllText(expectedPrettyPrintFile, ExpectedFormatted);

            var typeHintFile = Path.Combine(_testCasesDirectory, "settings.ini");
            var settingsFileContent = new[]
            {
                "[CSharp]",
                string.Format(
                    "{0}={1}",
                    ExternalTestSetting.SerializedType,
                    ExpectedSerializedType.AssemblyQualifiedName)
            };
            File.WriteAllLines(typeHintFile, settingsFileContent);
        }
    }
}
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
using KaVE.Commons.TestUtils.ExternalTests;
using NUnit.Framework;

namespace KaVE.Commons.Tests.ExternalTests
{
    internal class ExternalTestCaseProviderTest
    {
        private string _baseDirectory;

        private const string ExpectedFirstName = @"TestSuite\TestCases\FirstTest";
        private const string ExpectedFirstInput = "firstInputContent";
        private const string ExpectedCompact = "expectedCompactContent";
        private const string ExpectedPretty = "expectedPrettyContent";
        private static readonly Type ExpectedSerializedType = typeof (CompletionEvent);

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
            var testCases = ExternalTestCaseProvider.GetTestCases(_baseDirectory);
            Assert.AreEqual(2, testCases.Count());
        }

        [Test]
        public void ShouldFindInput()
        {
            var firstTestCase = ExternalTestCaseProvider.GetTestCases(_baseDirectory).First();
            Assert.AreEqual(ExpectedFirstInput, firstTestCase.Input);
        }

        [Test]
        public void ShouldFindCompactExpected()
        {
            var firstTestCase = ExternalTestCaseProvider.GetTestCases(_baseDirectory).First();
            Assert.AreEqual(ExpectedCompact, firstTestCase.ExpectedCompact);
        }

        [Test]
        public void ShouldFindPrettyExpected()
        {
            var firstTestCase = ExternalTestCaseProvider.GetTestCases(_baseDirectory).First();
            Assert.AreEqual(ExpectedPretty, firstTestCase.ExpectedPrettyPrint);
        }

        [Test]
        public void ShouldFindName()
        {
            var firstTestCase = ExternalTestCaseProvider.GetTestCases(_baseDirectory).First();
            Assert.AreEqual(ExpectedFirstName, firstTestCase.Name);
        }

        [Test]
        public void ShouldGetSerializedTypeFromTypeHint()
        {
            var firstTestCase = ExternalTestCaseProvider.GetTestCases(_baseDirectory).First();
            Assert.AreEqual(ExpectedSerializedType, firstTestCase.SerializedType);
        }

        private static void GenerateTestCaseStructure(string baseDirectory)
        {
            Directory.CreateDirectory(baseDirectory);

            var suiteBasePath = Path.Combine(baseDirectory, "TestSuite");
            Directory.CreateDirectory(suiteBasePath);

            var testCasesDirectory = Path.Combine(suiteBasePath, "TestCases");
            Directory.CreateDirectory(testCasesDirectory);

            var firstInputFile = Path.Combine(testCasesDirectory, "FirstTest.json");
            File.WriteAllText(firstInputFile, ExpectedFirstInput);

            var secondInputFile = Path.Combine(testCasesDirectory, "SecondTest.json");
            File.WriteAllText(secondInputFile, "secondInputContent");

            var expectedCompactFile = Path.Combine(testCasesDirectory, "expected-compact.json");
            File.WriteAllText(expectedCompactFile, ExpectedCompact);

            var expectedPrettyPrintFile = Path.Combine(testCasesDirectory, "expected-pretty.json");
            File.WriteAllText(expectedPrettyPrintFile, ExpectedPretty);

            var typeHintFile = Path.Combine(testCasesDirectory, "settings.ini");
            var settingsFileContent = new[]
            {
                "[CSharp]",
                string.Format("{0}={1}", ExternalTestSetting.SerializedType, ExpectedSerializedType.AssemblyQualifiedName)
            };
            File.WriteAllLines(typeHintFile, settingsFileContent);
        }
    }
}
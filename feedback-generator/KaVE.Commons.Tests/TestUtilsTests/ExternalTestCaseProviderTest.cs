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
using KaVE.Commons.TestUtils.ExternalTests;
using KaVE.Commons.Utils.Json;
using NUnit.Framework;

namespace KaVE.Commons.Tests.TestUtilsTests
{
    internal class ExternalTestCaseProviderTest
    {
        private string _baseDirectory;

        private const string ExpectedFirstName = @"TestSuite\TestCases\FirstTest";
        private const string ExpectedFirstInput = "firstInputContent";
        private const string ExpectedExpected = "expectedContent";

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
        public void ShouldFindInputAndExpected()
        {
            var firstTestCase = ExternalTestCaseProvider.GetTestCases(_baseDirectory).First();
            Assert.AreEqual(ExpectedFirstInput, firstTestCase.Input);
        }

        [Test]
        public void ShouldFindExpected()
        {
            var firstTestCase = ExternalTestCaseProvider.GetTestCases(_baseDirectory).First();
            Assert.AreEqual(ExpectedExpected, firstTestCase.Expected);
        }

        [Test]
        public void ShouldFindName()
        {
            var firstTestCase = ExternalTestCaseProvider.GetTestCases(_baseDirectory).First();
            Assert.AreEqual(ExpectedFirstName, firstTestCase.Name);
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

            var expectedFile = Path.Combine(testCasesDirectory, "expected.json");
            File.WriteAllText(expectedFile, ExpectedExpected);
        }
    }
}
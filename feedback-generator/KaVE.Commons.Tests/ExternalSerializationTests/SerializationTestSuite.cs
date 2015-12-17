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
using KaVE.Commons.Utils.Json;
using NUnit.Framework;

namespace KaVE.Commons.Tests.ExternalSerializationTests
{
    internal class SerializationTestSuite
    {
        private const string TestSourceRootFolder = @"..\..\ExternalSerializationTests\Data";

        private static IEnumerable<TestCase> TestCases
        {
            get { return TestCaseProvider.GetTestCases(TestSourceRootFolder); }
        }

        [TestCaseSource("TestCases")]
        public void StringEquality_Compact(TestCase testCase)
        {
            var actualString = testCase.Input.ParseJsonTo(testCase.SerializedType).ToCompactJson();
            Assert.AreEqual(testCase.ExpectedCompact, actualString);
        }

        [TestCaseSource("TestCases")]
        public void ObjectEquality_Compact(TestCase testCase)
        {
            var parsedInput = testCase.Input.ParseJsonTo(testCase.SerializedType);
            var parsedExpected = testCase.ExpectedCompact.ParseJsonTo(testCase.SerializedType);
            Assert.AreEqual(parsedExpected, parsedInput);
        }

        [TestCaseSource("TestCases")]
        public void StringEquality_Formatted(TestCase testCase)
        {
            if (testCase.ExpectedFormatted == null)
            {
                Assert.Ignore("No ExpectedFormatted");
            }

            var actualString = testCase.Input.ParseJsonTo(testCase.SerializedType).ToFormattedJson();
            Assert.AreEqual(testCase.ExpectedFormatted, actualString);
        }

        [TestCaseSource("TestCases")]
        public void ObjectEquality_Formatted(TestCase testCase)
        {
            if (testCase.ExpectedFormatted == null)
            {
                Assert.Ignore("No ExpectedFormatted");
            }

            var parsedInput = testCase.Input.ParseJsonTo(testCase.SerializedType);
            var parsedExpected = testCase.ExpectedFormatted.ParseJsonTo(testCase.SerializedType);
            Assert.AreEqual(parsedExpected, parsedInput);
        }

        [TestCaseSource("TestCases")]
        public void AssertEqualityOfExpectationFiles(TestCase testCase)
        {
            if (testCase.ExpectedFormatted == null)
            {
                Assert.Ignore("No ExpectedFormatted");
            }

            var parsedCompact = testCase.ExpectedCompact.ParseJsonTo(testCase.SerializedType);
            var parsedPrettyPrint = testCase.ExpectedFormatted.ParseJsonTo(testCase.SerializedType);
            Assert.AreEqual(parsedCompact, parsedPrettyPrint);
        }
    }
}
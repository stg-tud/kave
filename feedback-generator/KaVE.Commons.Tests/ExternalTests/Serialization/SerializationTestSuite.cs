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
using KaVE.Commons.TestUtils.ExternalTests;
using KaVE.Commons.Utils.Json;
using NUnit.Framework;

namespace KaVE.Commons.Tests.ExternalTests.Serialization
{
    [Ignore]
    internal class SerializationTestSuite
    {
        private const string TestSourceRootFolder = @"E:\KaVE\SerializationTests";

        private static IEnumerable<TestCase> TestCases
        {
            get { return ExternalTestCaseProvider.GetTestCases(TestSourceRootFolder); }
        }

        [TestCaseSource("TestCases")]
        public void Compact_StringEquality(TestCase testCase)
        {
            var actualString = testCase.Input.ParseJsonTo(testCase.SerializedType).ToCompactJson();
            Assert.AreEqual(testCase.ExpectedCompact, actualString);
        }

        [TestCaseSource("TestCases")]
        public void Compact_ObjectEquality(TestCase testCase)
        {
            var parsedInput = testCase.Input.ParseJsonTo(testCase.SerializedType);
            var parsedExpected = testCase.ExpectedCompact.ParseJsonTo(testCase.SerializedType);
            Assert.AreEqual(parsedExpected, parsedInput);
        }

        [TestCaseSource("TestCases")]
        public void PrettyPrint_StringEquality(TestCase testCase)
        {
            var actualString = testCase.Input.ParseJsonTo(testCase.SerializedType).ToPrettyPrintJson();
            Assert.AreEqual(testCase.ExpectedPrettyPrint, actualString);
        }

        [TestCaseSource("TestCases")]
        public void PrettyPrint_ObjectEquality(TestCase testCase)
        {
            var parsedInput = testCase.Input.ParseJsonTo(testCase.SerializedType);
            var parsedExpected = testCase.ExpectedPrettyPrint.ParseJsonTo(testCase.SerializedType);
            Assert.AreEqual(parsedExpected, parsedInput);
        }

        [TestCaseSource("TestCases")]
        public void PrettyPrint_Compact_ObjectEquality(TestCase testCase)
        {
            var parsedCompact = testCase.ExpectedCompact.ParseJsonTo(testCase.SerializedType);
            var parsedPrettyPrint = testCase.ExpectedPrettyPrint.ParseJsonTo(testCase.SerializedType);
            Assert.AreEqual(parsedCompact, parsedPrettyPrint);
        }
    }
}
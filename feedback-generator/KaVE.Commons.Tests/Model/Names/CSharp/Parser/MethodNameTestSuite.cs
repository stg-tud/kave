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
using System.Collections.Generic;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.Names.CSharp.Parser;
using KaVE.Commons.Tests.Model.Names.CSharp.Parser.Data;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Names.CSharp.Parser
{
    public class MethodNameTestSuite : TestCaseBaseTestSuite
    {
        private static IEnumerable<MethodNameTestCase> TestCases
        {
            get { return TestCaseProvider.ValidMethodNames(); }
        }
        private static IEnumerable<string> InvalidTestCases
        {
            get { return TestCaseProvider.InvalidMethodNames(); }
        }

        [Test, TestCaseSource("TestCases")]
        public void ValidMethodName(MethodNameTestCase testCase)
        {
            var type = CsNameUtil.GetMethodName(testCase.Identifier);
            Console.WriteLine(testCase.Identifier);
            Assert.AreEqual(testCase.DeclaringType, type.DeclaringType.Identifier);
            Assert.AreEqual(testCase.ReturnType, type.ReturnType.Identifier);
            AssertStrings(testCase.Parameters, type.Parameters);
            AssertStrings(testCase.TypeParameters, type.TypeParameters);
            Assert.AreEqual(testCase.IsStatic, type.IsStatic);
            Assert.AreEqual(testCase.IsGeneric, type.IsGenericEntity);
            Assert.AreEqual(testCase.SimpleName, type.Name);
        }

        [Test, TestCaseSource("InvalidTestCases")]
        public void InvalidMethodName(string invalidType)
        {
            Assert.AreEqual("?", CsNameUtil.GetMethodName(invalidType).Identifier);
        }
    }
}
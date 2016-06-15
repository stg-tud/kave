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
using KaVE.Commons.Tests.Model.Names.CSharp.Parser.Data;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Names.CSharp.Parser
{
    [TestFixture]
    public class ArrayTypeNameTestSuite : TestCaseBaseTestSuite
    {
        private static IEnumerable<ArrayTypeNameTestCase> TestCases
        {
            get { return TestCaseProvider.ValidArrayTypes(); }
        }
        private static IEnumerable<string> InvalidTestCases
        {
            get { return TestCaseProvider.InvalidArrayTypes(); }
        }

        [Test, TestCaseSource("TestCases")]
        public void ValidTypeName(ArrayTypeNameTestCase testCase)
        {
            var type = CsNameUtil.ParseTypeName(testCase.Identifier);
            Console.WriteLine(testCase.Identifier);
            Assert.NotNull(type);
            Assert.AreEqual(testCase.Identifier, type.Identifier);
            Assert.AreEqual(testCase.Assembly, type.Assembly.Identifier);
            Assert.AreEqual(testCase.Namespace, type.Namespace.Identifier);
            Assert.AreEqual(testCase.FullName, type.FullName);
            Assert.AreEqual(testCase.Name, type.Name);
            Assert.AreEqual(testCase.ArrayBaseType, type.ArrayBaseType.Identifier);
            AssertStrings(testCase.TypeParameters, type.TypeParameters);
            Assert.AreEqual(testCase.IsInterfaceType, type.IsInterfaceType);
            Assert.AreEqual(testCase.IsEnumType, type.IsEnumType);
            Assert.AreEqual(testCase.IsStructType, type.IsStructType);
            Assert.AreEqual(testCase.IsNestedType, type.IsNestedType);
            Assert.AreEqual(testCase.IsDelgateType, type.IsDelegateType);
            Assert.AreEqual(testCase.IsGenericEntity, type.IsGenericEntity);
        }

        [Test, TestCaseSource("InvalidTestCases")]
        public void InvalidTypeName(string invalidType)
        {
            Assert.AreEqual(CsNameUtil.ParseTypeName(invalidType).Identifier, "?");
        }
    }
}
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
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v1.Parser
{
    class TypeNameTestSuite : TestCaseBaseTestSuite
    {
        private static IEnumerable<TypeNameTestCase> TestCases
        {
            get { return TestCaseProvider.ValidTypeNames(); }
        }

        private static IEnumerable<string> InvalidTestCases
        {
            get { return TestCaseProvider.InvalidTypeNames(); }
        }

        [Test, TestCaseSource("TestCases")]
        public void ValidTypeName(TypeNameTestCase testCase)
        {
            var type = Commons.Model.Naming.Names.Type(testCase.Identifier);
            Console.WriteLine(testCase.Identifier);
            Assert.AreEqual(testCase.Identifier, type.Identifier);
            Assert.AreEqual(testCase.Assembly, type.Assembly.Identifier);
            Assert.AreEqual(testCase.Namespace, type.Namespace.Identifier);
            Assert.AreEqual(testCase.FullName, type.FullName);
            Assert.AreEqual(testCase.Name, type.Name);
            AssertStrings(testCase.TypeParameters, type.TypeParameters);
            Assert.AreEqual(testCase.IsInterfaceType, type.IsInterfaceType);
            Assert.AreEqual(testCase.IsEnumType, type.IsEnumType);
            Assert.AreEqual(testCase.IsStructType, type.IsStructType);
            Assert.AreEqual(testCase.IsNestedType, type.IsNestedType);
            Assert.AreEqual(testCase.IsReferenceType, type.IsReferenceType);
            Assert.AreEqual(testCase.IsClassType, type.IsClassType);
        }

        [Test, TestCaseSource("InvalidTestCases")]
        public void InvalidTypeName(string invalidType)
        {
            Assert.AreEqual(Commons.Model.Naming.Names.Type(invalidType).Identifier, "?");
        }
    }
}
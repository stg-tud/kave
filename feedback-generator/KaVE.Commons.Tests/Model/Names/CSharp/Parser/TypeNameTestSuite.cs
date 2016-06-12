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
using System.Linq;
using System.Text;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.Names.CSharp.Parser;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Names.CSharp.Parser
{
    [TestFixture]
    class TypeNameTestSuite
    {
        private static IEnumerable<TypeNameTestCase> TestCases
        {
            get { return TypeNameTestCaseProvider.ValidTypeNames(); }
        }
        private static IEnumerable<string> InvalidTestCases
        {
            get { return TypeNameTestCaseProvider.InvalidTypeNames(); }
        }

        [Test, TestCaseSource("TestCases")]
        public void ValidTypeName(TypeNameTestCase testCase)
        {
            Assert.DoesNotThrow(delegate { CsNameUtil.ParseTypeName(testCase.Identifier); });
            var type = new CsTypeName(testCase.Identifier);
            Console.WriteLine(testCase.Identifier);
            Assert.AreEqual(testCase.Identifier, type.Identifier);
            Assert.AreEqual(testCase.Assembly, type.Assembly.Identifier);
            Assert.AreEqual(testCase.Namespace, type.Namespace.Identifier);
            Assert.AreEqual(testCase.DeclaringType, type.DeclaringType.Identifier);
        }

        [Test, TestCaseSource("InvalidTestCases")]
        public void InvalidTypeName(string invalidType)
        {
            Assert.Catch(delegate { new CsTypeName(invalidType); });
        }

    }
}
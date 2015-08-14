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

using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Assertion;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.ObjectUsage
{
    internal class CoReMethodNameTest
    {
        [Test]
        public void ShouldRecognizeEqualMethodNames()
        {
            Assert.AreEqual(
                new CoReMethodName("LReceiver.method(LArgument;)LReturn;"),
                new CoReMethodName("LReceiver.method(LArgument;)LReturn;"));
        }

        [TestCase(
            "[System.String, mscore, 4.0.0.0] [MyType, MyAssembly, 1.0.0.0].AMethod([System.Int32, mscore, 4.0.0.0] length)"
            ), TestCase("KaVE.Model.ObjectUsage.Query.method(arg)"), TestCase("LSystem/Console.WriteLine()LSystem/Void"),
         TestCase("LType.method(LArgument)LReturn;"),
         ExpectedException(typeof (AssertException))]
        public void ShouldRejectInvalidMethodNames(string methodName)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new CoReMethodName(methodName);
        }

        [TestCase("LType.method(LArgument;)LReturn;"),
         TestCase("LT.m(LA;)LU;"),
         TestCase("LSystem/Console.WriteLine()LSystem/Void;"),
         TestCase("LP/T._M()LQ/T;"),
         TestCase("LP/T.M_N()LQ/T;"),
         TestCase("LICSharpCode/NRefactory/CSharp/CodeIssues/ConvertIfStatementToSwitchStatementIssueTests.TestτooSimpleCase1()LSystem/Void;")]
        public void ShouldAcceptValidMethodNames(string methodName)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new CoReMethodName(methodName);
        }

        [TestCase("LType.Method()LReturn;", "Method"), TestCase("LType.Method(LArg;)LReturn;", "Method"),
         TestCase("LType.Method(LArg1;LArg2;)LReturn;", "Method")]
        public void ShouldExtractMethodnameFromCoReMethodName(string origin, string expected)
        {
            var actual = new CoReMethodName(origin).Method;

            Assert.AreEqual(expected, actual);
        }
    }
}
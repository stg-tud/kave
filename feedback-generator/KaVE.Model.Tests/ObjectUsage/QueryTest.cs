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
 * 
 * Contributors:
 *    - Dennis Albrecht
 *    - Uli Fahrer
 */

using KaVE.Model.ObjectUsage;
using KaVE.Utils.Assertion;
using NUnit.Framework;

namespace KaVE.Model.Tests.ObjectUsage
{
    [TestFixture]
    internal class QueryTest
    {
        [Test]
        public void ShouldRecognizeEqualFieldNames()
        {
            Assert.AreEqual(
                new CoReFieldName("LField.field;LType"),
                new CoReFieldName("LField.field;LType"));
        }

        [Test]
        public void ShouldRecognizeEqualMethodNames()
        {
            Assert.AreEqual(
                new CoReMethodName("LReceiver.method(LArgument;)LReturn;"),
                new CoReMethodName("LReceiver.method(LArgument;)LReturn;"));
        }

        [Test]
        public void ShouldRecognizeEqualTypeNames()
        {
            Assert.AreEqual(
                new CoReTypeName("LClass"),
                new CoReTypeName("LClass"));
        }

        [Test]
        public void ShouldRecognizeEqualCallSites()
        {
            Assert.AreEqual(
                CallSites.CreateParameterCallSite("LReceiver.method(LArgument;)LReturn;", 2),
                CallSites.CreateParameterCallSite("LReceiver.method(LArgument;)LReturn;", 2));
        }

        [Test]
        public void ShouldRecognizeEqualDefinitionSites()
        {
            Assert.AreEqual(
                new DefinitionSite
                {
                    kind = DefinitionSiteKind.RETURN,
                    method = new CoReMethodName("LReceiver.method(LArgument;)LReturn;"),
                    field = new CoReFieldName("LField.field;LType"),
                    argIndex = 42
                },
                new DefinitionSite
                {
                    kind = DefinitionSiteKind.RETURN,
                    method = new CoReMethodName("LReceiver.method(LArgument;)LReturn;"),
                    field = new CoReFieldName("LField.field;LType"),
                    argIndex = 42
                });
        }

        [Test]
        public void ShouldRecognizeEqualQuerys()
        {
            var expected = new Query
            {
                type = new CoReTypeName("LType"),
                definition = DefinitionSites.CreateDefinitionByReturn("LFactory.method()LType;"),
                classCtx = new CoReTypeName("LClass"),
                methodCtx = new CoReMethodName("LReceiver.method(LArgument;)LReturn;"),
            };

            expected.sites.Add(CallSites.CreateParameterCallSite("LReceiver.method(LType;)LReturn;", 3));
            expected.sites.Add(CallSites.CreateParameterCallSite("LType.method(LArgument;)LReturn;", 0));

            var actual = new Query
            {
                type = new CoReTypeName("LType"),
                definition = DefinitionSites.CreateDefinitionByReturn("LFactory.method()LType;"),
                classCtx = new CoReTypeName("LClass"),
                methodCtx = new CoReMethodName("LReceiver.method(LArgument;)LReturn;")
            };

            actual.sites.Add(CallSites.CreateParameterCallSite("LReceiver.method(LType;)LReturn;", 3));
            actual.sites.Add(CallSites.CreateParameterCallSite("LType.method(LArgument;)LReturn;", 0));

            Assert.AreEqual(expected, actual);
        }

        [TestCase("System.Int32, mscore, 4.0.0.0"), TestCase("KaVE.Model.ObjectUsage.Query"), TestCase("Type"),
         TestCase("LType;"), TestCase("L"), TestCase("LN.T"),
         ExpectedException(typeof (AssertException))]
        public void ShouldRejectInvalidTypeNames(string typeName)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new CoReTypeName(typeName);
        }

        [TestCase("LType"), TestCase("LKaVE/Model/ObjectUsage/Query"), TestCase("LT1"), TestCase("LT$"),
         TestCase("LN1/T"), TestCase("[LType"), TestCase("[[[LType")]
        public void ShouldAcceptValidTypeNames(string typeName)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new CoReTypeName(typeName);
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
         TestCase("LSystem/Console.WriteLine()LSystem/Void;")]
        public void ShouldAcceptValidMethodNames(string methodName)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new CoReMethodName(methodName);
        }

        [TestCase("[System.Int32, mscore, 4.0.0.0] [MyClass, MyAssembly, 1.2.3.4].Constant"),
         TestCase("KaVE.Model.ObjectUsage.Query.type"), TestCase("LType.field;LType;"),
         ExpectedException(typeof (AssertException))]
        public void ShouldRejectInvalidFieldNames(string fieldName)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new CoReFieldName(fieldName);
        }

        [TestCase("LType.field;LType"),
         TestCase("LKaVE/Model/ObjectUsage/Query.type;LKave/Model/ObjectUsage/CoReTypeName")]
        public void ShouldAcceptValidFieldNames(string fieldName)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new CoReFieldName(fieldName);
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
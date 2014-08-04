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
                new CoReFieldName("Lcc/recommender/Field.field;Lcc/recommender/Type"),
                new CoReFieldName("Lcc/recommender/Field.field;Lcc/recommender/Type"));
        }

        [Test]
        public void ShouldRecognizeEqualMethodNames()
        {
            Assert.AreEqual(
                new CoReMethodName(
                    "Lcc/recommenders/Receiver.method(Lcc/recommenders/Argument;)Lcc/recommeners/Return;"),
                new CoReMethodName(
                    "Lcc/recommenders/Receiver.method(Lcc/recommenders/Argument;)Lcc/recommeners/Return;"));
        }

        [Test]
        public void ShouldRecognizeEqualTypeNames()
        {
            Assert.AreEqual(
                new CoReTypeName("Lcc/recommenders/Class"),
                new CoReTypeName("Lcc/recommenders/Class"));
        }

        [Test]
        public void ShouldRecognizeEqualCallSites()
        {
            Assert.AreEqual(
                new CallSite
                {
                    kind = CallSiteKind.PARAM_CALL_SITE,
                    call =
                        new CoReMethodName(
                            "Lcc/recommenders/Receiver.method(Lcc/recommenders/Argument;)Lcc/recommeners/Return;"),
                    argumentIndex = 2
                },
                new CallSite
                {
                    kind = CallSiteKind.PARAM_CALL_SITE,
                    call =
                        new CoReMethodName(
                            "Lcc/recommenders/Receiver.method(Lcc/recommenders/Argument;)Lcc/recommeners/Return;"),
                    argumentIndex = 2
                });
        }

        [Test]
        public void ShouldRecognizeEqualDefinitionSites()
        {
            Assert.AreEqual(
                new DefinitionSite
                {
                    kind = DefinitionKind.RETURN,
                    type = new CoReTypeName("Lcc/recommenders/Class"),
                    method =
                        new CoReMethodName(
                            "Lcc/recommenders/Receiver.method(Lcc/recommenders/Argument;)Lcc/recommeners/Return;"),
                    field = new CoReFieldName("Lcc/recommender/Field.field;Lcc/recommender/Type"),
                    arg = 42
                },
                new DefinitionSite
                {
                    kind = DefinitionKind.RETURN,
                    type = new CoReTypeName("Lcc/recommenders/Class"),
                    method =
                        new CoReMethodName(
                            "Lcc/recommenders/Receiver.method(Lcc/recommenders/Argument;)Lcc/recommeners/Return;"),
                    field = new CoReFieldName("Lcc/recommender/Field.field;Lcc/recommender/Type"),
                    arg = 42
                });
        }

        [Test]
        public void ShouldRecognizeEqualQuerys()
        {
            var expected = new Query
            {
                type = new CoReTypeName("Lcc/recommenders/Type"),
                definition = new DefinitionSite
                {
                    kind = DefinitionKind.RETURN,
                    method = new CoReMethodName("Lcc/recommenders/Factory.method()Lcc/recommeners/Type;")
                },
                classCtx = new CoReTypeName("Lcc/recommenders/Class"),
                methodCtx =
                    new CoReMethodName(
                        "Lcc/recommenders/Receiver.method(Lcc/recommenders/Argument;)Lcc/recommeners/Return;"),
            };
            expected.sites.Add(
                new CallSite
                {
                    kind = CallSiteKind.PARAM_CALL_SITE,
                    call =
                        new CoReMethodName(
                            "Lcc/recommenders/Receiver.method(Lcc/recommenders/Type;)Lcc/recommeners/Return;"),
                    argumentIndex = 3
                });
            expected.sites.Add(
                new CallSite
                {
                    kind = CallSiteKind.RECEIVER_CALL_SITE,
                    call =
                        new CoReMethodName(
                            "Lcc/recommenders/Type.method(Lcc/recommenders/Argument;)Lcc/recommeners/Return;"),
                    argumentIndex = 0
                });

            var actual = new Query
            {
                type = new CoReTypeName("Lcc/recommenders/Type"),
                definition = new DefinitionSite
                {
                    kind = DefinitionKind.RETURN,
                    method = new CoReMethodName("Lcc/recommenders/Factory.method()Lcc/recommeners/Type;")
                },
                classCtx = new CoReTypeName("Lcc/recommenders/Class"),
                methodCtx =
                    new CoReMethodName(
                        "Lcc/recommenders/Receiver.method(Lcc/recommenders/Argument;)Lcc/recommeners/Return;")
            };
            actual.sites.Add(
                new CallSite
                {
                    kind = CallSiteKind.PARAM_CALL_SITE,
                    call =
                        new CoReMethodName(
                            "Lcc/recommenders/Receiver.method(Lcc/recommenders/Type;)Lcc/recommeners/Return;"),
                    argumentIndex = 3
                });
            actual.sites.Add(
                new CallSite
                {
                    kind = CallSiteKind.RECEIVER_CALL_SITE,
                    call =
                        new CoReMethodName(
                            "Lcc/recommenders/Type.method(Lcc/recommenders/Argument;)Lcc/recommeners/Return;"),
                    argumentIndex = 0
                });

            Assert.AreEqual(
                expected,
                actual);
        }

        [TestCase("System.Int32, mscore, 4.0.0.0"), TestCase("KaVE.Model.ObjectUsage.Query"),
         ExpectedException(typeof (AssertException))]
        public void ShouldRejectInvalidTypeNames(string typeName)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new CoReTypeName(typeName);
        }

        [TestCase("Lcc/recommenders/Type"), TestCase("LKaVE/Model/ObjectUsage/Query")]
        public void ShouldAcceptValidTypeNames(string typeName)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new CoReTypeName(typeName);
        }

        [TestCase(
            "[System.String, mscore, 4.0.0.0] [MyType, MyAssembly, 1.0.0.0].AMethod([System.Int32, mscore, 4.0.0.0] length)"
            ), TestCase("KaVE.Model.ObjectUsage.Query.method(arg)"),
         ExpectedException(typeof (AssertException))]
        public void ShouldRejectInvalidMethodNames(string typeName)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new CoReMethodName(typeName);
        }

        [TestCase("Lcc/recommenders/Type.method(Lcc/recommenders/Argument;)Lcc/recommeners/Return;"),
         TestCase("LSystem/Console.WriteLine()LSystem/Void;")]
        public void ShouldAcceptValidMethodNames(string typeName)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new CoReMethodName(typeName);
        }

        [TestCase("[System.Int32, mscore, 4.0.0.0] [MyClass, MyAssembly, 1.2.3.4].Constant"),
         TestCase("KaVE.Model.ObjectUsage.Query.type"),
         ExpectedException(typeof (AssertException))]
        public void ShouldRejectInvalidFieldNames(string typeName)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new CoReFieldName(typeName);
        }

        [TestCase("Lcc/recommenders/Type.field;Lcc/recommenders/Type"),
         TestCase("LKaVE/Model/ObjectUsage/Query.type;LKave/Model/ObjectUsage/CoReTypeName")]
        public void ShouldAcceptValidFieldNames(string typeName)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new CoReFieldName(typeName);
        }
    }
}
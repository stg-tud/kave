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
 *    - Roman Fojtik
 *    - Sebastian Proksch
 */

using System.Collections.ObjectModel;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using NUnit.Framework;
using Fix = KaVE.Commons.Tests.Utils.ObjectUsageExporterTestSuite.ObjectUsageExporterTestFixture;

namespace KaVE.Commons.Tests.Utils.ObjectUsageExporterTestSuite
{
    /// <summary>
    ///     tests variable name look-ups, call registration on correct types, etc.
    /// </summary>
    public class InvocationCollectionTest : BaseObjectUsageExporterTest
    {
        [Test]
        public void AmbiguousCallsOnUndeclaredVariable() {}

        [Test]
        public void CallOnSubtypeIsRegisteredForVariableType()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
                InvokeStmt("a", Method(Fix.Void, Type("A"), "M1")),
                InvokeStmt("a", Method(Fix.Void, Type("B"), "M2")));

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(
                            Method(Fix.Void, Type("B"), "MethodTB")),
                        CallSites.CreateReceiverCallSite(
                            Method(Fix.Void, Type("A"), "MethodTA"))
                    }
                });
        }

        [Test]
        public void CallOnSubtypeIsRegisteredForSuperType_EvenWhenTypeExists() {}

        [Test]
        public void AnalysisOfMultipleMethods()
        {
            Context = new Context
            {
                SST = new SST
                {
                    EnclosingType = Type("Test"),
                    Methods =
                    {
                        new MethodDeclaration
                        {
                            Name = Method(Fix.Void, Type("Test"), "M1"),
                            Body =
                            {
                                VarDecl("a", Type("A")),
                                Assign("a", Constructor(Type("A"))),
                                InvokeStmt("a", Method(Fix.Void, Type("A"), "A1"))
                            }
                        },
                        new MethodDeclaration
                        {
                            Name = Method(Fix.Void, Type("Test"), "M2"),
                            Body =
                            {
                                VarDecl("a", Type("A")),
                                Assign("a", Constructor(Type("A"))),
                                InvokeStmt("a", Method(Fix.Void, Type("A"), "A2"))
                            }
                        }
                    }
                }
            };

            var actual = Sut.Export(Context);

            var expected = new Collection<Query>
            {
                new Query
                {
                    type = Type("A").ToCoReName(),
                    classCtx = Type("Test").ToCoReName(),
                    methodCtx = Method(Fix.Void, Type("Test"), "M1").ToCoReName(),
                    definition = DefinitionSites.CreateDefinitionByConstructor(Method(Fix.Void, Type("A"), ".ctor")),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Fix.Void, Type("A"), "A1"))
                    }
                },
                new Query
                {
                    type = Type("A").ToCoReName(),
                    classCtx = Type("Test").ToCoReName(),
                    methodCtx = Method(Fix.Void, Type("Test"), "M2").ToCoReName(),
                    definition = DefinitionSites.CreateDefinitionByConstructor(Method(Fix.Void, Type("A"), ".ctor")),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Fix.Void, Type("A"), "A2"))
                    }
                }
            };

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TypeOfThisUsages_SuperTypeIfAvailable() {}

        [Test]
        public void TypeOfThisUsages_ElementTypeIfNoSuperIsAvailable() {}
    }
}
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

using System.Collections.ObjectModel;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.TypeShapes;
using NUnit.Framework;
using Fix = KaVE.Commons.Tests.Utils.ObjectUsageExport.UsageExtractorTestSuite.ObjectUsageExporterTestFixture;

namespace KaVE.Commons.Tests.Utils.ObjectUsageExport.UsageExtractorTestSuite
{
    /// <summary>
    ///     tests variable name look-ups, call registration on correct types, etc.
    /// </summary>
    public class InvocationCollectionTest : BaseObjectUsageExporterTest
    {
        [Test]
        public void AmbiguousCallsOnUndeclaredVariable()
        {
            SetupDefaultEnclosingMethod(
                InvokeStmt("a", Method(Fix.Void, Type("A"), "MA")),
                InvokeStmt("a", Method(Fix.Void, Type("B"), "MB")));

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(
                            Method(Fix.Void, Type("A"), "MA"))
                    }
                },
                new Query
                {
                    type = Type("B").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(
                            Method(Fix.Void, Type("B"), "MB"))
                    }
                });
        }

        [Test]
        public void CallOnSubtypeIsRegisteredForVariableType()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
                InvokeStmt("a", Method(Fix.Void, Type("A"), "MA")),
                InvokeStmt("a", Method(Fix.Void, Type("B"), "MB")));

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(
                            Method(Fix.Void, Type("A"), "MA")),
                        CallSites.CreateReceiverCallSite(
                            Method(Fix.Void, Type("B"), "MB"))
                    }
                });
        }

        [Test]
        public void CallOnSubtypeIsRegisteredForSuperType_EvenWhenTypeExists()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
                VarDecl("b", Type("B")),
                InvokeStmt("a", Method(Fix.Void, Type("B"), "M")));

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(
                            Method(Fix.Void, Type("B"), "M"))
                    }
                });
        }

        [Test]
        public void AnalysisOfMultipleMethods()
        {
            Context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = new TypeHierarchy
                    {
                        Element = Type("Test")
                    }
                },
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
        public void TypeOfThisUsages_ElementTypeIfNoSuperIsAvailable()
        {
            SetupDefaultEnclosingMethod(
                InvokeStmt("this", Method(Fix.Void, DefaultClassContext, "M")));

            Context.TypeShape.TypeHierarchy = new TypeHierarchy
            {
                Element = DefaultClassContext
            };

            AssertQueriesWithoutSettingContexts(
                new Query
                {
                    type = DefaultClassContext.ToCoReName(),
                    classCtx = DefaultClassContext.ToCoReName(),
                    methodCtx = DefaultMethodContext.ToCoReName(),
                    definition = DefinitionSites.CreateDefinitionByThis(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(
                            Method(Fix.Void, DefaultClassContext, "M"))
                    }
                });
        }

        [Test]
        public void TypeOfThisUsages_SuperTypeIfAvailable()
        {
            SetupDefaultEnclosingMethod(
                InvokeStmt("this", Method(Fix.Void, DefaultClassContext, "M")));

            Context.TypeShape.TypeHierarchy = new TypeHierarchy
            {
                Element = DefaultClassContext,
                Extends = new TypeHierarchy
                {
                    Element = Type("TSuper")
                }
            };

            AssertQueriesWithoutSettingContexts(
                new Query
                {
                    type = Type("TSuper").ToCoReName(),
                    classCtx = Type("TSuper").ToCoReName(),
                    methodCtx = DefaultMethodContext.ToCoReName(),
                    definition = DefinitionSites.CreateDefinitionByThis(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(
                            Method(Fix.Void, DefaultClassContext, "M"))
                    }
                });
        }
    }
}
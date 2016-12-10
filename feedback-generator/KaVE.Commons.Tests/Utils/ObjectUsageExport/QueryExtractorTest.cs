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

using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Model.SSTs.Statements;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.ObjectUsageExport;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.ObjectUsageExport
{
    internal class QueryExtractorTest
    {
        private static ITypeName DefaultClassContext
        {
            get { return Names.Type("T,P"); }
        }

        private static IMethodName DefaultMethodContext
        {
            get { return Names.Method("[R,P] [T,P].M()"); }
        }

        private QueryExtractor _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new QueryExtractor();
        }

        [Test]
        public void HappyPath()
        {
            var context = NewContextWithDefaults(
                new TypeHierarchy
                {
                    Element = DefaultClassContext
                },
                new MethodHierarchy
                {
                    Element = DefaultMethodContext
                },
                VarDecl("A", "a"),
                InvokeStmt("a", Method("R", "A", "M")),
                Completion("a")
                );

            var actual = _sut.Extract(context);
            var expected = new Query
            {
                type = Type("A").ToCoReName(),
                classCtx = DefaultClassContext.ToCoReName(),
                methodCtx = DefaultMethodContext.ToCoReName(),
                definition = DefinitionSites.CreateUnknownDefinitionSite(),
                sites =
                {
                    CallSites.CreateReceiverCallSite(Method("R", "A", "M"))
                }
            };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NoCompletionFound()
        {
            var context = NewContextWithDefaults(
                new TypeHierarchy
                {
                    Element = DefaultClassContext
                },
                new MethodHierarchy
                {
                    Element = DefaultMethodContext
                },
                VarDecl("A", "a"),
                InvokeStmt("a", Method("R", "A", "M"))
                );

            var actual = _sut.Extract(context);
            Assert.Null(actual);
        }

        [Test]
        public void ReuseWithNoCompletionFound()
        {
            var context = NewContextWithDefaults(
                new TypeHierarchy
                {
                    Element = DefaultClassContext
                },
                new MethodHierarchy
                {
                    Element = DefaultMethodContext
                },
                VarDecl("A", "a"),
                InvokeStmt("a", Method("R", "A", "M")),
                Completion("a")
                );

            _sut.Extract(context);

            context = NewContextWithDefaults(
                new TypeHierarchy
                {
                    Element = DefaultClassContext
                },
                new MethodHierarchy
                {
                    Element = DefaultMethodContext
                },
                VarDecl("A", "a"),
                InvokeStmt("a", Method("R", "A", "M"))
                );

            var actual = _sut.Extract(context);

            Assert.IsNull(actual);
        }

        [Test]
        public void ContextRewrite()
        {
            // exemplary test, this is extensively tested for usage export

            var context = NewContextWithDefaults(
                new TypeHierarchy
                {
                    Element = DefaultClassContext,
                    Extends = new TypeHierarchy
                    {
                        Element = Type("TSuper")
                    }
                },
                new MethodHierarchy
                {
                    Element = DefaultMethodContext
                },
                VarDecl("A", "a"),
                InvokeStmt("a", Method("R", "A", "M")),
                Completion("a")
                );

            var actual = _sut.Extract(context);
            var expected = new Query
            {
                type = Type("A").ToCoReName(),
                classCtx = Type("TSuper").ToCoReName(),
                methodCtx = DefaultMethodContext.ToCoReName(),
                definition = DefinitionSites.CreateUnknownDefinitionSite(),
                sites =
                {
                    CallSites.CreateReceiverCallSite(Method("R", "A", "M"))
                }
            };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TriggerOnThis_NoBaseClass()
        {
            var context = NewContextWithDefaults(
                new TypeHierarchy
                {
                    Element = DefaultClassContext
                },
                new MethodHierarchy
                {
                    Element = DefaultMethodContext
                },
                InvokeStmt("this", Method("R", "T", "M2")),
                Completion("this")
                );

            var actual = _sut.Extract(context);
            var expected = new Query
            {
                type = DefaultClassContext.ToCoReName(),
                classCtx = DefaultClassContext.ToCoReName(),
                methodCtx = DefaultMethodContext.ToCoReName(),
                definition = DefinitionSites.CreateDefinitionByThis(),
                sites =
                {
                    CallSites.CreateReceiverCallSite(Method("R", "T", "M2"))
                }
            };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TriggerOnThis_HasBaseClass()
        {
            var context = NewContextWithDefaults(
                new TypeHierarchy
                {
                    Element = DefaultClassContext,
                    Extends = new TypeHierarchy
                    {
                        Element = Type("TSuper")
                    }
                },
                new MethodHierarchy
                {
                    Element = DefaultMethodContext
                },
                InvokeStmt("this", Method("R", "T", "M2")),
                Completion("this")
                );

            var actual = _sut.Extract(context);
            var expected = new Query
            {
                type = Type("TSuper").ToCoReName(),
                classCtx = Type("TSuper").ToCoReName(),
                methodCtx = DefaultMethodContext.ToCoReName(),
                definition = DefinitionSites.CreateDefinitionByThis(),
                sites =
                {
                    CallSites.CreateReceiverCallSite(Method("R", "T", "M2"))
                }
            };
            Assert.AreEqual(expected, actual);
        }

        #region helper

        private static ITypeName Type(string typeName)
        {
            return Names.Type(typeName + ", P");
        }

        private static IMethodName Method(string returnTypeName, string declTypeName, string methodName)
        {
            return Names.Method(string.Format("[{0}, P] [{1}, P].{2}()", returnTypeName, declTypeName, methodName));
        }

        private static IVariableDeclaration VarDecl(string type, string id)
        {
            return new VariableDeclaration
            {
                Type = Type(type),
                Reference = new VariableReference
                {
                    Identifier = id
                }
            };
        }

        private static IStatement InvokeStmt(string id, IMethodName method)
        {
            return new ExpressionStatement
            {
                Expression = new InvocationExpression
                {
                    Reference = new VariableReference {Identifier = id},
                    MethodName = method
                }
            };
        }

        private static IStatement Completion(string id)
        {
            return new ExpressionStatement
            {
                Expression = new CompletionExpression
                {
                    VariableReference = new VariableReference
                    {
                        Identifier = id
                    }
                }
            };
        }

        private Context NewContextWithDefaults(ITypeHierarchy typeHierarchy,
            IMemberHierarchy<IMethodName> methodHierarchy,
            params IStatement[] statements)
        {
            return new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = typeHierarchy,
                    MethodHierarchies =
                    {
                        methodHierarchy
                    }
                },
                SST = new SST
                {
                    EnclosingType = Type("T"),
                    Methods =
                    {
                        new MethodDeclaration
                        {
                            Name = Method("R", "T", "M"),
                            Body = Lists.NewListFrom(statements)
                        }
                    }
                }
            };
        }

        #endregion
    }
}
/*
 * Copyright 2017 Sebastian Proksch
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
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Model.SSTs.Statements;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.StatisticsUltimate;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.StatisticsUltimate
{
    internal class ContextStatisticsExtractorTest
    {
        private ContextStatisticsExtractor _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new ContextStatisticsExtractor();
        }

        private IContextStatistics Extract(params Context[] contexts)
        {
            return _sut.Extract(contexts);
        }

        [Test]
        public void UserAndRepoIsLeftAlone()
        {
            var actual = _sut.Extract(
                new[]
                {
                    new Context
                    {
                        SST = new SST
                        {
                            EnclosingType = Names.Type("C,P")
                        }
                    }
                });
            Assert.AreEqual(0, actual.NumUsers);
            Assert.AreEqual(0, actual.NumRepositories);
        }

        [Test]
        public void SolutionIsOnlyCountedOnce()
        {
            var actual = _sut.Extract(
                new[]
                {
                    new Context
                    {
                        SST = new SST
                        {
                            EnclosingType = Names.Type("C1,P")
                        }
                    },
                    new Context
                    {
                        SST = new SST
                        {
                            EnclosingType = Names.Type("C2,P")
                        }
                    }
                });
            Assert.AreEqual(1, actual.NumSolutions);
        }

        [TestCase("C,P"), TestCase("i:I,P"), TestCase("s:S,P"), TestCase("e:E,P"), TestCase("d:[p:void] [D,P].()")]
        public void CountsTopLevelTypes(string id)
        {
            var actual = _sut.Extract(
                new[]
                {
                    new Context
                    {
                        SST = new SST
                        {
                            EnclosingType = Names.Type(id)
                        }
                    }
                });
            Assert.AreEqual(1, actual.NumTopLevelType);
        }

        [TestCase("T+C,P"), TestCase("i:T+I,P"), TestCase("s:T+S,P"), TestCase("e:T+E,P"),
         TestCase("d:[p:void] [T+D,P].()")]
        public void CountsNestedTypes(string nestedId)
        {
            var actual = _sut.Extract(
                new[]
                {
                    new Context
                    {
                        SST = new SST
                        {
                            EnclosingType = Names.Type(nestedId)
                        }
                    }
                });
            Assert.AreEqual(1, actual.NumNestedType);
        }

        [TestCase("C,P"), TestCase("T+C,P")]
        public void CountsClasses(string id)
        {
            var actual = Extract(
                new Context
                {
                    SST = new SST
                    {
                        EnclosingType = Names.Type(id)
                    }
                });
            Assert.AreEqual(1, actual.NumClasses);
        }

        [TestCase("i:I,P"), TestCase("i:T+I,P")]
        public void CountsInterfaces(string id)
        {
            var actual = Extract(
                new Context
                {
                    SST = new SST
                    {
                        EnclosingType = Names.Type(id)
                    }
                });
            Assert.AreEqual(1, actual.NumInterfaces);
        }

        [TestCase("d:[p:void] [D,P].()"), TestCase("d:[p:void] [T+D,P].()")]
        public void CountsDelegates(string id)
        {
            var actual = Extract(
                new Context
                {
                    SST = new SST
                    {
                        EnclosingType = Names.Type(id)
                    }
                });
            Assert.AreEqual(1, actual.NumDelegates);
        }

        [TestCase("s:S,P"), TestCase("s:T+S,P")]
        public void CountsStructs(string id)
        {
            var actual = Extract(
                new Context
                {
                    SST = new SST
                    {
                        EnclosingType = Names.Type(id)
                    }
                });
            Assert.AreEqual(1, actual.NumStructs);
        }

        [TestCase("e:E,P"), TestCase("e:T+E,P")]
        public void CountsEnums(string id)
        {
            var actual = Extract(
                new Context
                {
                    SST = new SST
                    {
                        EnclosingType = Names.Type(id)
                    }
                });
            Assert.AreEqual(1, actual.NumEnums);
        }

        [TestCase("p:int"), TestCase("T[],P"), ExpectedException(typeof(AssertException))]
        public void ShouldCrashOnUnusualDeclarations(string id)
        {
            Extract(
                new Context
                {
                    SST = new SST
                    {
                        EnclosingType = Names.Type(id)
                    }
                });
        }

        [Test]
        public void ShouldCountTypeHierarchy()
        {
            var actual = Extract(
                new Context
                {
                    TypeShape = new TypeShape
                    {
                        TypeHierarchy = new TypeHierarchy
                        {
                            Element = Names.Type("C1,P")
                        }
                    },
                    SST = new SST
                    {
                        EnclosingType = Names.Type("C1,P")
                    }
                },
                new Context
                {
                    TypeShape = new TypeShape
                    {
                        TypeHierarchy = new TypeHierarchy
                        {
                            Element = Names.Type("C2,P"),
                            Implements =
                            {
                                new TypeHierarchy("i:I2,P")
                            }
                        }
                    },
                    SST = new SST
                    {
                        EnclosingType = Names.Type("C2,P")
                    }
                },
                new Context
                {
                    TypeShape = new TypeShape
                    {
                        TypeHierarchy = new TypeHierarchy
                        {
                            Element = Names.Type("C3,P"),
                            Extends = new TypeHierarchy("S3,P")
                        }
                    },
                    SST = new SST
                    {
                        EnclosingType = Names.Type("C3,P")
                    }
                });
            Assert.AreEqual(3, actual.NumTopLevelType);
            Assert.AreEqual(2, actual.NumClassExtendOrImplement);
        }

        [Test]
        public void ShouldNotCountTypeHierarchiesTwice()
        {
            var actual = Extract(
                new Context
                {
                    TypeShape = new TypeShape
                    {
                        TypeHierarchy = new TypeHierarchy
                        {
                            Element = Names.Type("C,P"),
                            Extends = new TypeHierarchy("S,P"),
                            Implements =
                            {
                                new TypeHierarchy("i:I,P")
                            }
                        }
                    },
                    SST = new SST
                    {
                        EnclosingType = Names.Type("C,P")
                    }
                });
            Assert.AreEqual(1, actual.NumClassExtendOrImplement);
        }

        [Test]
        public void ShouldCountMethodDeclarations()
        {
            var actual = Extract(
                new Context
                {
                    SST = new SST
                    {
                        EnclosingType = Names.Type("C,P"),
                        Methods =
                        {
                            new MethodDeclaration()
                        }
                    }
                });
            Assert.AreEqual(1, actual.NumMethodDecls);
        }

        [Test]
        public void ShouldCountMethodHierarchies()
        {
            var actual = Extract(
                new Context
                {
                    SST = new SST
                    {
                        EnclosingType = Names.Type("C,P"),
                        Methods =
                        {
                            new MethodDeclaration {Name = Names.Method("[p:void] [T,P].M()")}
                        }
                    }
                },
                new Context
                {
                    TypeShape = new TypeShape
                    {
                        MethodHierarchies =
                        {
                            new MethodHierarchy
                            {
                                Element = Names.Method("[p:void] [T,P].M()"),
                                Super = Names.Method("[p:void] [S,P].M()")
                            }
                        }
                    },
                    SST = new SST
                    {
                        EnclosingType = Names.Type("C,P"),
                        Methods =
                        {
                            new MethodDeclaration {Name = Names.Method("[p:void] [T,P].M()")}
                        }
                    }
                },
                new Context
                {
                    TypeShape = new TypeShape
                    {
                        MethodHierarchies =
                        {
                            new MethodHierarchy
                            {
                                Element = Names.Method("[p:void] [T,P].M()"),
                                First = Names.Method("[p:void] [F,P].M()")
                            }
                        }
                    },
                    SST = new SST
                    {
                        EnclosingType = Names.Type("C,P"),
                        Methods =
                        {
                            new MethodDeclaration {Name = Names.Method("[p:void] [T,P].M()")}
                        }
                    }
                });

            Assert.AreEqual(3, actual.NumMethodDecls);
            Assert.AreEqual(2, actual.NumMethodOverridesOrImplements);
        }

        [Test]
        public void ShouldNotCountMethodHierarchiesTwice()
        {
            var actual = Extract(
                new Context
                {
                    TypeShape = new TypeShape
                    {
                        MethodHierarchies =
                        {
                            new MethodHierarchy
                            {
                                Element = Names.Method("[p:void] [T,P].M()"),
                                Super = Names.Method("[p:void] [S,P].M()"),
                                First = Names.Method("[p:void] [F,P].M()")
                            }
                        }
                    },
                    SST = new SST
                    {
                        EnclosingType = Names.Type("C,P"),
                        Methods =
                        {
                            new MethodDeclaration {Name = Names.Method("[p:void] [T,P].M()")}
                        }
                    }
                });

            Assert.AreEqual(1, actual.NumMethodDecls);
        }

        [Test]
        public void ShouldCountMethodCallsAndRegisterAssembly()
        {
            var actual =
                Extract(
                    CreateContextWithSSTAndMethodBody(
                        InvStmt("[p:void] [T,A,1.2.3.4].M()"),
                        InvStmt("[p:void] [T,A,1.2.3.4].M()")));

            Assert.AreEqual(2, actual.NumAsmCalls);
            Assert.AreEqual(Sets.NewHashSet(Names.Method("[p:void] [T,A,1.2.3.4].M()")), actual.UniqueAsmMethods);
            Assert.AreEqual(Sets.NewHashSet(Names.Assembly("A,1.2.3.4")), actual.UniqueAssemblies);
        }

        [Test]
        public void ShouldNotCountLocalMethodCallsAndRegisterAssembly()
        {
            var actual = Extract(CreateContextWithSSTAndMethodBody(InvStmt("[p:void] [T,P].M()")));

            Assert.AreEqual(0, actual.NumAsmCalls);
            Assert.AreEqual(Sets.NewHashSet<IMethodName>(), actual.UniqueAsmMethods);
            Assert.AreEqual(Sets.NewHashSet<IAssemblyName>(), actual.UniqueAssemblies);
        }

        [Test]
        public void ShouldCountMethodRefsAndRegisterAssembly()
        {
            var actual =
                Extract(
                    CreateContextWithSSTAndMethodBody(
                        Stmt(RefExpr(new MethodReference {MethodName = Names.Method("[p:void] [T,A,1.2.3.4].M()")}))));

            Assert.AreEqual(0, actual.NumAsmCalls);
            Assert.AreEqual(Sets.NewHashSet(Names.Method("[p:void] [T,A,1.2.3.4].M()")), actual.UniqueAsmMethods);
            Assert.AreEqual(Sets.NewHashSet(Names.Assembly("A,1.2.3.4")), actual.UniqueAssemblies);
        }

        [Test]
        public void ShouldNotCountLocalMethodRefsAndRegisterAssembly()
        {
            var actual =
                Extract(
                    CreateContextWithSSTAndMethodBody(
                        Stmt(RefExpr(new MethodReference {MethodName = Names.Method("[p:void] [T,P].M()")}))));

            Assert.AreEqual(0, actual.NumAsmCalls);
            Assert.AreEqual(Sets.NewHashSet<IMethodName>(), actual.UniqueAsmMethods);
            Assert.AreEqual(Sets.NewHashSet<IAssemblyName>(), actual.UniqueAssemblies);
        }

        [Test]
        public void ShouldCountFieldRefsAndRegisterAssembly()
        {
            var actual =
                Extract(
                    CreateContextWithSSTAndMethodBody(
                        Stmt(RefExpr(new FieldReference {FieldName = Names.Field("[p:int] [T,A,1.2.3.4]._f")}))));

            Assert.AreEqual(1, actual.NumAsmFieldRead);
            Assert.AreEqual(Sets.NewHashSet(Names.Field("[p:int] [T,A,1.2.3.4]._f")), actual.UniqueAsmFields);
            Assert.AreEqual(Sets.NewHashSet(Names.Assembly("A,1.2.3.4")), actual.UniqueAssemblies);
        }

        [Test]
        public void ShouldNotCountLocalFieldRefsAndRegisterAssembly()
        {
            var actual =
                Extract(
                    CreateContextWithSSTAndMethodBody(
                        Stmt(RefExpr(new FieldReference {FieldName = Names.Field("[p:int] [T,P]._f")}))));

            Assert.AreEqual(0, actual.NumAsmFieldRead);
            Assert.AreEqual(Sets.NewHashSet<IFieldName>(), actual.UniqueAsmFields);
            Assert.AreEqual(Sets.NewHashSet<IAssemblyName>(), actual.UniqueAssemblies);
        }

        [Test]
        public void ShouldCountPropertyRefsAndRegisterAssembly()
        {
            var actual =
                Extract(
                    CreateContextWithSSTAndMethodBody(
                        Stmt(
                            RefExpr(
                                new PropertyReference
                                {
                                    PropertyName = Names.Property("get set [p:int] [T,A,1.2.3.4].P()")
                                }))));

            Assert.AreEqual(1, actual.NumAsmPropertyRead);
            Assert.AreEqual(
                Sets.NewHashSet(Names.Property("get set [p:int] [T,A,1.2.3.4].P()")),
                actual.UniqueAsmProperties);
            Assert.AreEqual(Sets.NewHashSet(Names.Assembly("A,1.2.3.4")), actual.UniqueAssemblies);
        }

        [Test]
        public void ShouldNotCountLocalPropertyRefsAndRegisterAssembly()
        {
            var actual =
                Extract(
                    CreateContextWithSSTAndMethodBody(
                        Stmt(
                            RefExpr(new PropertyReference {PropertyName = Names.Property("get set [p:int] [T,P].P()")}))));

            Assert.AreEqual(0, actual.NumAsmPropertyRead);
            Assert.AreEqual(Sets.NewHashSet<IPropertyName>(), actual.UniqueAsmProperties);
            Assert.AreEqual(Sets.NewHashSet<IAssemblyName>(), actual.UniqueAssemblies);
        }

        [Test]
        public void ShouldCountLinesOfCode()
        {
            var actual = Extract(
                // 3 (class, method decl, inv)
                CreateContextWithSSTAndMethodBody(InvStmt("[p:void] [T,P].M1()")),
                // 4 (class, method decl, 2x inv)
                CreateContextWithSSTAndMethodBody(InvStmt("[p:void] [T,P].M2()"), InvStmt("[p:void] [T,P].M2()")));

            Assert.AreEqual(7, actual.EstimatedLinesOfCode);
        }

        private static IExpressionStatement InvStmt(string methodId)
        {
            return Stmt(
                new InvocationExpression
                {
                    MethodName = Names.Method(methodId)
                }
            );
        }

        private static IExpressionStatement Stmt(IAssignableExpression expr)
        {
            return new ExpressionStatement
            {
                Expression = expr
            };
        }

        private IReferenceExpression RefExpr(IReference aref)
        {
            return new ReferenceExpression
            {
                Reference = aref
            };
        }

        private static Context CreateContextWithSSTAndMethodBody(params IStatement[] stmts)
        {
            return new Context
            {
                SST = new SST
                {
                    EnclosingType = Names.Type("C,P"),
                    Methods =
                    {
                        new MethodDeclaration
                        {
                            Name = Names.Method("[p:void] [T,P].M()"),
                            Body = Lists.NewListFrom(stmts)
                        }
                    }
                }
            };
        }
    }
}
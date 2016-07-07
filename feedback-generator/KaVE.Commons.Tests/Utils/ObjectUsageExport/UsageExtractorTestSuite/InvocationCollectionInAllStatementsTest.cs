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

using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Model.SSTs.Impl.Blocks;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.LoopHeader;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using NUnit.Framework;
using Fix = KaVE.Commons.Tests.Utils.ObjectUsageExport.UsageExtractorTestSuite.ObjectUsageExporterTestFixture;

namespace KaVE.Commons.Tests.Utils.ObjectUsageExport.UsageExtractorTestSuite
{
    /// <summary>
    ///     tests if all statement types are covered in the call site collection
    /// </summary>
    internal class InvocationCollectionInAllStatementsTest : BaseObjectUsageExporterTest
    {
        [Test]
        public void AssignmentTest()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("i", Type("I")),
                Assign("i", Invoke("this", Method(Type("I"), DefaultClassContext, "M"))),
                InvokeStmt("i", SomeMethodOnType("I")));


            AssertQueriesInDefault(
                new Query
                {
                    type = DefaultClassContext.ToCoReName(),
                    definition = DefinitionSites.CreateDefinitionByThis(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("I"), DefaultClassContext, "M"))
                    }
                },
                new Query
                {
                    type = Type("I").ToCoReName(),
                    definition = DefinitionSites.CreateDefinitionByReturn(Method(Type("I"), DefaultClassContext, "M")),
                    sites =
                    {
                        SomeCallSiteOnType("I")
                    }
                }
                );
        }

        [Test]
        public void ExpressionTest()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
                new ExpressionStatement
                {
                    Expression = new InvocationExpression
                    {
                        MethodName = Method(Type("R"), Type("A"), "methodA"),
                        Reference = VarRef("a")
                    }
                });

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("R"), Type("A"), "methodA"))
                    }
                });
        }

        [Test]
        public void LabelledStatementTest()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
                new LabelledStatement
                {
                    Statement = new ExpressionStatement
                    {
                        Expression = new InvocationExpression
                        {
                            MethodName = Method(Type("R"), Type("A"), "methodA"),
                            Reference = VarRef("a")
                        }
                    }
                });

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("R"), Type("A"), "methodA"))
                    }
                });
        }

        [Test]
        public void LambdaExpressionTest()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("a", Fix.Action),
                Assign(
                    "a",
                    new LambdaExpression
                    {
                        Name = Names.Lambda("[{0}] ()", Fix.Void),
                        Body =
                        {
                            VarDecl("v", Type("V")),
                            new ExpressionStatement
                            {
                                Expression = new InvocationExpression
                                {
                                    MethodName = Method(Type("R"), Type("V"), "methodV"),
                                    Reference = VarRef("v")
                                }
                            }
                        }
                    }));


            AssertQueriesWithoutSettingContexts(
                new Query
                {
                    type = Type("V").ToCoReName(),
                    classCtx = Type("TDecl$Lambda").ToCoReName(),
                    methodCtx = Names.Method("[{0}] [{1}].M$Lambda()", Type("A"), DefaultClassContext).ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("R"), Type("V"), "methodV"))
                    }
                });
        }

        [Test]
        public void DoLoopCondition()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
                new DoLoop
                {
                    Condition = new LoopHeaderBlockExpression
                    {
                        Body =
                        {
                            new ExpressionStatement
                            {
                                Expression = new InvocationExpression
                                {
                                    MethodName = Method(Type("R"), Type("A"), "methodA"),
                                    Reference = VarRef("a")
                                }
                            }
                        }
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("R"), Type("A"), "methodA"))
                    }
                });
        }

        [Test]
        public void DoLoopBody()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("b", Type("B")),
                new DoLoop
                {
                    Body =
                    {
                        new ExpressionStatement
                        {
                            Expression = new InvocationExpression
                            {
                                MethodName = Method(Type("R"), Type("B"), "methodB"),
                                Reference = VarRef("b")
                            }
                        }
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("B").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("R"), Type("B"), "methodB"))
                    }
                });
        }

        [Test]
        public void WhileLoopCondition()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
                new WhileLoop
                {
                    Condition = new LoopHeaderBlockExpression
                    {
                        Body =
                        {
                            new ExpressionStatement
                            {
                                Expression = new InvocationExpression
                                {
                                    MethodName = Method(Type("R"), Type("A"), "methodA"),
                                    Reference = VarRef("a")
                                }
                            }
                        }
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("R"), Type("A"), "methodA"))
                    }
                });
        }

        [Test]
        public void WhileLoopAndBody()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("b", Type("B")),
                new WhileLoop
                {
                    Body =
                    {
                        new ExpressionStatement
                        {
                            Expression = new InvocationExpression
                            {
                                MethodName = Method(Type("R"), Type("B"), "methodB"),
                                Reference = VarRef("b")
                            }
                        }
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("B").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("R"), Type("B"), "methodB"))
                    }
                });
        }

        [Test]
        public void ForEachLoopDeclaration()
        {
            SetupDefaultEnclosingMethod(
                new ForEachLoop
                {
                    Declaration = VarDecl("b", Type("B")),
                    Body =
                    {
                        InvokeStmt("b", Method(Fix.Void, Type("B"), "M"))
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("B").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Fix.Void, Type("B"), "M"))
                    }
                });
        }

        [Test]
        public void ForEachLoopBody()
        {
            SetupDefaultEnclosingMethod(
                new ForEachLoop
                {
                    Body =
                    {
                        VarDecl("b", Type("B")),
                        new ExpressionStatement
                        {
                            Expression = new InvocationExpression
                            {
                                MethodName = Method(Type("R"), Type("B"), "methodB"),
                                Reference = VarRef("b")
                            }
                        }
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("B").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("R"), Type("B"), "methodB"))
                    }
                });
        }

        [Test]
        public void ForLoopCondition()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
                new ForLoop
                {
                    Condition = new LoopHeaderBlockExpression
                    {
                        Body =
                        {
                            new ExpressionStatement
                            {
                                Expression = new InvocationExpression
                                {
                                    MethodName = Method(Type("R"), Type("A"), "methodA"),
                                    Reference = VarRef("a")
                                }
                            }
                        }
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("R"), Type("A"), "methodA"))
                    }
                });
        }

        [Test]
        public void ForLoopBody()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("b", Type("B")),
                new ForLoop
                {
                    Body =
                    {
                        new ExpressionStatement
                        {
                            Expression = new InvocationExpression
                            {
                                MethodName = Method(Type("R"), Type("B"), "methodB"),
                                Reference = VarRef("b")
                            }
                        }
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("B").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("R"), Type("B"), "methodB"))
                    }
                });
        }

        [Test]
        public void ForLoopInit()
        {
            SetupDefaultEnclosingMethod(
                new ForLoop
                {
                    Init =
                    {
                        VarDecl("a", Type("A")),
                        new ExpressionStatement
                        {
                            Expression = new InvocationExpression
                            {
                                MethodName = Method(Type("R"), Type("A"), "methodA"),
                                Reference = VarRef("a")
                            }
                        }
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("R"), Type("A"), "methodA"))
                    }
                });
        }

        [Test]
        public void ForLoopStep()
        {
            SetupDefaultEnclosingMethod(
                new ForLoop
                {
                    Step =
                    {
                        VarDecl("b", Type("B")),
                        new ExpressionStatement
                        {
                            Expression = new InvocationExpression
                            {
                                MethodName = Method(Type("R"), Type("B"), "methodB"),
                                Reference = VarRef("b")
                            }
                        }
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("B").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("R"), Type("B"), "methodB"))
                    }
                });
        }

        [Test]
        public void IfElseBlockThen()
        {
            SetupDefaultEnclosingMethod(
                new IfElseBlock
                {
                    Then =
                    {
                        VarDecl("a", Type("A")),
                        new ExpressionStatement
                        {
                            Expression = new InvocationExpression
                            {
                                MethodName = Method(Type("R"), Type("A"), "methodA"),
                                Reference = VarRef("a")
                            }
                        }
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("R"), Type("A"), "methodA"))
                    }
                });
        }

        [Test]
        public void IfElseBlockElse()
        {
            SetupDefaultEnclosingMethod(
                new IfElseBlock
                {
                    Else =
                    {
                        VarDecl("b", Type("B")),
                        new ExpressionStatement
                        {
                            Expression = new InvocationExpression
                            {
                                MethodName = Method(Type("R"), Type("B"), "methodB"),
                                Reference = VarRef("b")
                            }
                        }
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("B").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("R"), Type("B"), "methodB"))
                    }
                });
        }

        [Test]
        public void LockBlock()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("b", Type("B")),
                new LockBlock
                {
                    Reference = VarRef("a"),
                    Body =
                    {
                        new ExpressionStatement
                        {
                            Expression = new InvocationExpression
                            {
                                MethodName = Method(Type("R"), Type("B"), "methodB"),
                                Reference = VarRef("b")
                            }
                        }
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("B").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("R"), Type("B"), "methodB"))
                    }
                });
        }

        [Test]
        public void SwitchBlockSections()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
                new SwitchBlock
                {
                    Reference = VarRef("a"),
                    Sections =
                    {
                        new CaseBlock
                        {
                            Body =
                            {
                                new ExpressionStatement
                                {
                                    Expression = new InvocationExpression
                                    {
                                        MethodName = Method(Type("R"), Type("A"), "methodA"),
                                        Reference = VarRef("a")
                                    }
                                }
                            }
                        }
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("R"), Type("A"), "methodA"))
                    }
                });
        }

        [Test]
        public void SwitchBlockDefaultSection()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("b", Type("B")),
                new SwitchBlock
                {
                    Reference = VarRef("a"),
                    DefaultSection =
                    {
                        new ExpressionStatement
                        {
                            Expression = new InvocationExpression
                            {
                                MethodName = Method(Type("R"), Type("B"), "methodB"),
                                Reference = VarRef("b")
                            }
                        }
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("B").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("R"), Type("B"), "methodB"))
                    }
                });
        }

        [Test]
        public void TryBlockBody()
        {
            SetupDefaultEnclosingMethod(
                new TryBlock
                {
                    Body =
                    {
                        VarDecl("a", Type("A")),
                        new ExpressionStatement
                        {
                            Expression = new InvocationExpression
                            {
                                MethodName = Method(Type("R"), Type("A"), "methodA"),
                                Reference = VarRef("a")
                            }
                        }
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("R"), Type("A"), "methodA"))
                    }
                });
        }

        [Test]
        public void TryBlockFinally()
        {
            SetupDefaultEnclosingMethod(
                new TryBlock
                {
                    Finally =
                    {
                        VarDecl("b", Type("B")),
                        new ExpressionStatement
                        {
                            Expression = new InvocationExpression
                            {
                                MethodName = Method(Type("R"), Type("B"), "methodB"),
                                Reference = VarRef("b")
                            }
                        }
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("B").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("R"), Type("B"), "methodB"))
                    }
                });
        }

        [Test]
        public void TryBlockCatchBlockBody()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
                new TryBlock
                {
                    CatchBlocks =
                    {
                        new CatchBlock
                        {
                            Body =
                            {
                                InvokeStmt("a", Method(Fix.Void, Type("A"), "M"))
                            }
                        }
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    definition = DefinitionSites.CreateUnknownDefinitionSite(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Fix.Void, Type("A"), "M"))
                    }
                });
        }

        [Test]
        public void TryBlockCatchBlockParameter()
        {
            SetupDefaultEnclosingMethod(
                new TryBlock
                {
                    CatchBlocks =
                    {
                        new CatchBlock
                        {
                            Parameter = Parameter(Type("T"), "p"),
                            Body =
                            {
                                InvokeStmt("p", Method(Fix.Void, Type("T"), "M"))
                            }
                        }
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("T").ToCoReName(),
                    definition = DefinitionSites.CreateUnknownDefinitionSite(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Fix.Void, Type("T"), "M"))
                    }
                });
        }


        [Test]
        public void UncheckedBlock()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("b", Type("B")),
                new UncheckedBlock
                {
                    Body =
                    {
                        new ExpressionStatement
                        {
                            Expression = new InvocationExpression
                            {
                                MethodName = Method(Type("R"), Type("B"), "methodB"),
                                Reference = VarRef("b")
                            }
                        }
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("B").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("R"), Type("B"), "methodB"))
                    }
                });
        }

        [Test]
        public void UsingBlock()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("b", Type("B")),
                new UsingBlock
                {
                    Body =
                    {
                        new ExpressionStatement
                        {
                            Expression = new InvocationExpression
                            {
                                MethodName = Method(Type("R"), Type("B"), "methodB"),
                                Reference = VarRef("b")
                            }
                        }
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("B").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("R"), Type("B"), "methodB"))
                    }
                });
        }
    }
}
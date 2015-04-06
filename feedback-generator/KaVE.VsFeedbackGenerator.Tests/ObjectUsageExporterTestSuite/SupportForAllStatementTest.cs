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
 */

using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Model.SSTs.Impl.Blocks;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.LoopHeader;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.ObjectUsageExporterTestSuite
{
    internal class SupportForAllStatementTest : BaseObjectUsageExporterTest
    {
        //// statements
        //void Visit(IAssignment stmt, TContext context);           x
        //void Visit(IExpressionStatement stmt, TContext context);  x
        //void Visit(IGotoStatement stmt, TContext context);        -
        //void Visit(ILabelledStatement stmt, TContext context);    x
        //void Visit(IReturnStatement stmt, TContext context);      -
        //void Visit(IThrowStatement stmt, TContext context);       -

        //// blocks
        //void Visit(IDoLoop block, TContext context);          x
        //void Visit(IWhileLoop block, TContext context);       x
        //void Visit(IForEachLoop block, TContext context);     x
        //void Visit(IForLoop block, TContext context);         x
        //void Visit(IIfElseBlock block, TContext context);     x
        //void Visit(ILockBlock stmt, TContext context);        x
        //void Visit(ISwitchBlock block, TContext context);     x
        //void Visit(ITryBlock block, TContext context);        x
        //void Visit(IUncheckedBlock block, TContext context);  x
        //void Visit(IUnsafeBlock block, TContext context);     - 
        //void Visit(IUsingBlock block, TContext context);      x

        [Test]
        public void AssignmentTest()
        {
            SetupDefaultEnclosingMethod(
                new VariableDeclaration
                {
                    Reference = VarRef("i"),
                    Type = Type("I")
                },
                new Assignment
                {
                    Reference = VarRef("i"),
                    Expression = new InvocationExpression
                    {
                        MethodName = Method(Type("R"), Type("Decl"), "Method"),
                        Reference = VarRef("i")
                    }
                });

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("I").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("R"), Type("Decl"), "Method"))
                    }
                });
        }

        [Test]
        public void ExpressionTest()
        {
            SetupDefaultEnclosingMethod(
                new VariableDeclaration
                {
                    Reference = VarRef("a"),
                    Type = Type("A")
                },
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
                new VariableDeclaration
                {
                    Reference = VarRef("a"),
                    Type = Type("A")
                },
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
        public void DoLoopCondition()
        {
            SetupDefaultEnclosingMethod(
                new VariableDeclaration
                {
                    Reference = VarRef("a"),
                    Type = Type("A")
                },
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
                new VariableDeclaration
                {
                    Reference = VarRef("b"),
                    Type = Type("B")
                },
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
                new VariableDeclaration
                {
                    Reference = VarRef("a"),
                    Type = Type("A")
                },
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
                new VariableDeclaration
                {
                    Reference = VarRef("b"),
                    Type = Type("B")
                },
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
                    Declaration = new VariableDeclaration
                    {
                        Reference = VarRef("b"),
                        Type = Type("B")
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("B").ToCoReName()
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
                        new VariableDeclaration
                        {
                            Reference = VarRef("b"),
                            Type = Type("B")
                        },
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
                new VariableDeclaration
                {
                    Reference = VarRef("a"),
                    Type = Type("A")
                },
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
                new VariableDeclaration
                {
                    Reference = VarRef("b"),
                    Type = Type("B")
                },
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
                        new VariableDeclaration
                        {
                            Reference = VarRef("a"),
                            Type = Type("A")
                        },
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
                        new VariableDeclaration
                        {
                            Reference = VarRef("b"),
                            Type = Type("B")
                        },
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
                        new VariableDeclaration
                        {
                            Reference = VarRef("a"),
                            Type = Type("A")
                        },
                        new ExpressionStatement
                        {
                            Expression = new InvocationExpression
                            {
                                MethodName = Method(Type("R"), Type("A"), "methodA"),
                                Reference = VarRef("a")
                            }
                        },
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
                        new VariableDeclaration
                        {
                            Reference = VarRef("b"),
                            Type = Type("B")
                        },
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
                new VariableDeclaration
                {
                    Reference = VarRef("b"),
                    Type = Type("B")
                },
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
                new VariableDeclaration
                {
                    Reference = VarRef("a"),
                    Type = Type("A")
                },
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
                new VariableDeclaration
                {
                    Reference = VarRef("b"),
                    Type = Type("B")
                },
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
                        new VariableDeclaration
                        {
                            Reference = VarRef("a"),
                            Type = Type("A")
                        },
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
                        new VariableDeclaration
                        {
                            Reference = VarRef("b"),
                            Type = Type("B")
                        },
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
                new TryBlock
                {
                    CatchBlocks =
                    {
                        new CatchBlock
                        {
                            Body =
                            {
                                new VariableDeclaration
                                {
                                    Reference = VarRef("a"),
                                    Type = Type("A")
                                },
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
                },
                // query for unknown parameter
                new Query
                {
                    type = TypeName.UnknownName.ToCoReName()
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
                            Parameter = Parameter(Type("TParam"), "param")
                        }
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("TParam").ToCoReName()
                });
        }


        [Test]
        public void UncheckedBlock()
        {
            SetupDefaultEnclosingMethod(
                new VariableDeclaration
                {
                    Reference = VarRef("b"),
                    Type = Type("B")
                },
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
                new VariableDeclaration
                {
                    Reference = VarRef("b"),
                    Type = Type("B")
                },
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
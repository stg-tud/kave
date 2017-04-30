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
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Blocks;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.LoopHeader;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.EditLocation;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.EditLocation
{
    internal class RelativeEditLocationAnalysisTest
    {
        private RelativeEditLocationAnalysis _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new RelativeEditLocationAnalysis();
        }

        #region default cases

        [Test]
        public void HasEditLocation_FalseIfNotFound()
        {
            Analyze();
            AssertNoLocation();
        }

        [Test]
        public void HasEditLocation_TrueIfFound()
        {
            Analyze(
                new ExpressionStatement
                {
                    Expression = new CompletionExpression()
                });
            AssertLocation(1, 1);
        }

        [Test]
        public void Size_CompletionBefore()
        {
            Analyze(
                new ExpressionStatement
                {
                    Expression = new CompletionExpression()
                },
                new ContinueStatement()
            );
            AssertLocation(1, 2);
        }

        [Test]
        public void Size_CompletionAfter()
        {
            Analyze(
                new ContinueStatement(),
                new ExpressionStatement
                {
                    Expression = new CompletionExpression()
                });
            AssertLocation(2, 2);
        }

        #endregion

        #region blocks

        [Test]
        public void Blocks_DoLoop()
        {
            AssertForBlock(new DoLoop {Condition = CreateLoopHeaderBlockOfSize(1), Body = {SomeStatement()}}, 3);
        }

        [Test]
        public void Blocks_ForEachLoop()
        {
            AssertForBlock(new ForEachLoop {Body = {SomeStatement()}}, 2);
        }

        [Test]
        public void Blocks_ForLoop()
        {
            AssertForBlock(
                new ForLoop
                {
                    Init =
                    {
                        SomeStatement()
                    },
                    Condition = CreateLoopHeaderBlockOfSize(1),
                    Step =
                    {
                        SomeStatement()
                    },
                    Body =
                    {
                        SomeStatement()
                    }
                },
                5);
        }

        [Test]
        public void Blocks_IfElseBlock()
        {
            AssertForBlock(new IfElseBlock {Then = {SomeStatement()}, Else = {SomeStatement()}}, 3);
        }

        [Test]
        public void Blocks_LockBlock()
        {
            AssertForBlock(new LockBlock {Body = {SomeStatement()}}, 2);
        }

        [Test]
        public void Blocks_SwitchBlock()
        {
            AssertForBlock(
                new SwitchBlock
                {
                    DefaultSection = {SomeStatement()},
                    Sections =
                    {
                        new CaseBlock
                        {
                            Body =
                            {
                                SomeStatement()
                            }
                        }
                    }
                },
                3);
        }

        [Test]
        public void Blocks_TryBlock()
        {
            AssertForBlock(
                new TryBlock
                {
                    Body = {SomeStatement()},
                    CatchBlocks =
                    {
                        new CatchBlock
                        {
                            Body =
                            {
                                SomeStatement()
                            }
                        }
                    },
                    Finally = {SomeStatement()}
                },
                4);
        }

        [Test]
        public void Blocks_UncheckedBlock()
        {
            AssertForBlock(
                new UncheckedBlock
                {
                    Body = {SomeStatement()}
                },
                2);
        }

        [Test]
        public void Blocks_UnsafeBlock()
        {
            AssertForBlock(new UnsafeBlock(), 1);
        }

        [Test]
        public void Blocks_UsingBlock()
        {
            AssertForBlock(
                new UsingBlock
                {
                    Body = {SomeStatement()}
                },
                2);
        }

        [Test]
        public void Blocks_WhileLoop()
        {
            AssertForBlock(new WhileLoop {Condition = CreateLoopHeaderBlockOfSize(1), Body = {SomeStatement()}}, 3);
        }

        #endregion

        #region statements

        [Test]
        public void Statements_Assignment()
        {
            AssertSupportForStatement(new Assignment());
        }

        [Test]
        public void Statements_Assignment_Nested()
        {
            Analyze(
                new Assignment
                {
                    Expression = new CompletionExpression()
                });
            AssertLocation(1, 1);
        }

        [Test]
        public void Statements_BreakStatement()
        {
            AssertSupportForStatement(new BreakStatement());
        }

        [Test]
        public void Statements_ContinueStatement()
        {
            AssertSupportForStatement(new ContinueStatement());
        }

        [Test]
        public void Statements_EventSubscriptionStatement()
        {
            AssertSupportForStatement(new EventSubscriptionStatement());
        }

        [Test]
        public void Statements_EventSubscriptionStatement_Nested()
        {
            Analyze(
                new EventSubscriptionStatement
                {
                    Expression = new CompletionExpression()
                });
            AssertLocation(1, 1);
        }

        [Test]
        public void Statements_ExpressionStatement()
        {
            AssertSupportForStatement(new ExpressionStatement());
        }

        [Test]
        public void Statements_GotoStatement()
        {
            AssertSupportForStatement(new GotoStatement());
        }

        [Test]
        public void Statements_LabelledStatement()
        {
            AssertSupportForStatement(
                new LabelledStatement
                {
                    Statement = new ContinueStatement()
                });
        }

        [Test]
        public void Statements_LabelledStatement_Nested()
        {
            Analyze(
                new LabelledStatement
                {
                    Statement = new ExpressionStatement
                    {
                        Expression = new CompletionExpression()
                    }
                });
            AssertLocation(1, 1);
        }

        [Test]
        public void Statements_ReturnStatement()
        {
            AssertSupportForStatement(new ReturnStatement());
        }

        [Test]
        public void Statements_ThrowStatement()
        {
            AssertSupportForStatement(new ThrowStatement());
        }

        [Test]
        public void Statements_UnknownStatement()
        {
            AssertSupportForStatement(new UnknownStatement());
        }

        [Test]
        public void Statements_VariableDeclarationStatement()
        {
            AssertSupportForStatement(new VariableDeclaration());
        }

        #endregion

        #region nesting expressions

        [Test]
        public void Expression_EmptyLambdaExpression()
        {
            AssertSupportForStatement(
                new ExpressionStatement
                {
                    Expression = new LambdaExpression()
                });
        }

        [Test]
        public void Expression_LambdaExpression()
        {
            Analyze(
                new ExpressionStatement
                {
                    Expression = new LambdaExpression
                    {
                        Body =
                        {
                            new ExpressionStatement
                            {
                                Expression = new CompletionExpression()
                            }
                        }
                    }
                });
            AssertLocation(2, 2);
        }

        #endregion

        #region integration

        [Test]
        public void IntegrationTest1()
        {
            var sst = new SST
            {
                Methods =
                {
                    new MethodDeclaration
                    {
                        Body =
                        {
                            new VariableDeclaration
                            {
                                Reference = new VariableReference {Identifier = "coreName"},
                                Type = Names.Type("KaVE.Commons.Model.ObjectUsage.CoReName, KaVE.Commons, 1.0.0.0")
                            },
                            new Assignment
                            {
                                Reference = new VariableReference {Identifier = "coreName"},
                                Expression = new UnknownExpression()
                            },
                            new IfElseBlock
                            {
                                Condition = new UnknownExpression(),
                                Then =
                                {
                                    new ExpressionStatement
                                    {
                                        Expression = new InvocationExpression
                                        {
                                            Reference = new VariableReference {Identifier = "writer"},
                                            MethodName =
                                                Names.Method(
                                                    "[System.Void, mscorlib, 4.0.0.0] [Newtonsoft.Json.JsonWriter, Newtonsoft.Json, 6.0.0.0].WriteValue([System.String, mscorlib, 4.0.0.0] value)"),
                                            Parameters =
                                            {
                                                new ReferenceExpression
                                                {
                                                    Reference = new VariableReference {Identifier = "Name"}
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            new VariableDeclaration
                            {
                                Reference = new VariableReference {Identifier = "callSite"},
                                Type = Names.Type("KaVE.Commons.Model.ObjectUsage.CallSite, KaVE.Commons, 1.0.0.0")
                            },
                            new Assignment
                            {
                                Reference = new VariableReference {Identifier = "callSite"},
                                Expression = new UnknownExpression()
                            },
                            new ExpressionStatement
                            {
                                Expression = new CompletionExpression {Token = "i"}
                            }
                        }
                    }
                }
            };

            var actual = _sut.Analyze(sst);
            var expected = new RelativeEditLocation {Size = 7, Location = 7};
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region testhelper

        private RelativeEditLocation _actual;

        private void Analyze(params IStatement[] stmts)
        {
            var sst = new SST
            {
                Methods =
                {
                    new MethodDeclaration
                    {
                        Body = Lists.NewListFrom(stmts)
                    }
                }
            };
            _actual = _sut.Analyze(sst);
        }

        private void AssertNoLocation()
        {
            var expected = new RelativeEditLocation();
            Assert.AreEqual(expected, _actual);
        }

        private void AssertLocation(int expectedLocation, int expectedSize)
        {
            var expected = new RelativeEditLocation
            {
                Location = expectedLocation,
                Size = expectedSize
            };
            Assert.AreEqual(expected, _actual);
        }

        private void AssertSupportForStatement(IStatement stmt)
        {
            Analyze(
                stmt,
                new ExpressionStatement
                {
                    Expression = new CompletionExpression()
                });
            AssertLocation(2, 2);
        }

        private void AssertForBlock(IStatement stmt, int sizeOfBlock)
        {
            Analyze(
                stmt,
                new ExpressionStatement
                {
                    Expression = new CompletionExpression()
                });
            AssertLocation(sizeOfBlock + 1, sizeOfBlock + 1);
        }

        private static ContinueStatement SomeStatement()
        {
            return new ContinueStatement();
        }

        private static LoopHeaderBlockExpression CreateLoopHeaderBlockOfSize(int size)
        {
            var body = Lists.NewList<IStatement>();
            for (var i = 0; i < size; i++)
            {
                body.Add(new ContinueStatement());
            }
            return new LoopHeaderBlockExpression {Body = body};
        }

        #endregion
    }
}
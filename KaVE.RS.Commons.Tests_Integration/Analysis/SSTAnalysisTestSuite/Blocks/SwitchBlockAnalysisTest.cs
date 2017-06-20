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

using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Blocks;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Blocks
{
    internal class SwitchBlockAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void Empty()
        {
            CompleteInMethod(@"switch(this) { } $");

            AssertBody(
                new SwitchBlock {Reference = VarRef("this")},
                Fix.EmptyCompletion);
        }

        [Test]
        public void DefaultSectionOnly()
        {
            CompleteInMethod(@"
                switch (this)
                {
                    default:
                        break;
                }
                $");

            AssertBody(
                new SwitchBlock {Reference = VarRef("this"), DefaultSection = {new BreakStatement()}},
                Fix.EmptyCompletion);
        }

        [Test]
        public void MultiLabel()
        {
            CompleteInMethod(@"
                switch (this)
                {
                    case 0:
                    case 1:
                        break;
                }
                $");

            AssertBody(
                new SwitchBlock
                {
                    Reference = VarRef("this"),
                    Sections =
                    {
                        new CaseBlock
                        {
                            Label = Const("0")
                        },
                        new CaseBlock
                        {
                            Label = Const("1"),
                            Body = {new BreakStatement()}
                        }
                    }
                },
                Fix.EmptyCompletion);
        }

        [Test]
        public void ExprLabel()
        {
            CompleteInMethod(@"
                const int l = 4;
                switch ()
                {
                    case 1*2:
                        continue;
                    case 3:
                        break;
                    case l:
                        continue;
                        break
                }
                $");

            AssertBody(
                VarDecl("l", Fix.Int),
                Assign("l", new ConstantValueExpression()),
                VarDecl("$0", Fix.Int),
                Assign(
                    "$0",
                    new BinaryExpression
                    {
                        LeftOperand = Const("1"),
                        Operator = BinaryOperator.Multiply,
                        RightOperand = Const("2")
                    }),
                new SwitchBlock
                {
                    Sections =
                    {
                        new CaseBlock
                        {
                            Label = RefExpr("$0"),
                            Body = {new ContinueStatement()}
                        },
                        new CaseBlock
                        {
                            Label = new ConstantValueExpression(),
                            Body = {new BreakStatement()}
                        },
                        new CaseBlock
                        {
                            Label = RefExpr("l"),
                            Body = {new ContinueStatement(), new BreakStatement()}
                        }
                    }
                },
                Fix.EmptyCompletion);
        }

        [Test]
        public void Standard()
        {
            CompleteInMethod(@"
                switch (this)
                {
                    case 0:
                    case 1:
                        break;

                    case 2:
                        break;
                        break;

                    default:
                        break;
                        break;
                        break;
                }
                $");

            AssertBody(
                new SwitchBlock
                {
                    Reference = VarRef("this"),
                    Sections =
                    {
                        new CaseBlock {Label = Const("0")},
                        new CaseBlock {Label = Const("1"), Body = {new BreakStatement()}},
                        new CaseBlock
                        {
                            Label = Const("2"),
                            Body =
                            {
                                new BreakStatement(),
                                new BreakStatement()
                            }
                        }
                    },
                    DefaultSection = {new BreakStatement(), new BreakStatement(), new BreakStatement()}
                },
                Fix.EmptyCompletion);
        }

        [Test]
        public void CompletionInCaseBlock_First()
        {
            CompleteInMethod(@"
                switch ()
                {
                    default:
                        $
                        break;
                }");

            AssertBody(
                new SwitchBlock
                {
                    DefaultSection = {Fix.EmptyCompletion, new BreakStatement()}
                });
        }

        [Test]
        public void CompletionInCaseBlock_Second()
        {
            CompleteInMethod(@"
                switch ()
                {
                    default:
                        break;
                        $
                }");

            AssertBody(
                new SwitchBlock
                {
                    DefaultSection = {new BreakStatement(), Fix.EmptyCompletion}
                });
        }

        [Test]
        public void CompletionOutsideCaseBlockIsIgnored_BeforeFirst()
        {
            CompleteInMethod(@"
                switch ()
                {
                    $

                    default:
                        break;
                }");

            AssertBody(
                new SwitchBlock
                {
                    DefaultSection = {new BreakStatement()}
                });
        }

        [Test]
        public void CompletionOutsideCaseBlockIsIgnored_Empty()
        {
            CompleteInMethod(@"
                switch ()
                {
                    $
                }");

            AssertBody(new SwitchBlock());
        }

        [Test]
        public void CompletionInSwitchBlock_Before()
        {
            CompleteInMethod(@"
                $
                switch ()
                {
                    default:
                        break;
                }");

            AssertBody(
                Fix.EmptyCompletion,
                new SwitchBlock
                {
                    DefaultSection = {new BreakStatement()}
                });
        }

        [Test]
        public void CompletionInSwitchBlock_AfterLabel()
        {
            CompleteInMethod(@"
                switch ()
                {
                    default:
                        $
                }");

            AssertBody(
                new SwitchBlock
                {
                    DefaultSection = {Fix.EmptyCompletion}
                });
        }

        [Test]
        public void CompletionInSwitchBlock_AfterLabelMulti()
        {
            CompleteInMethod(@"
                switch ()
                {
                    case 0:
                        $
                    case 1:
                        continue;
                }");

            AssertBody(
                new SwitchBlock
                {
                    Sections =
                    {
                        new CaseBlock {Label = Const("0"), Body = {Fix.EmptyCompletion}},
                        new CaseBlock {Label = Const("1"), Body = {new ContinueStatement()}}
                    }
                });
        }

        [Test]
        public void CompletionInSwitchBlock_AfterLabelNonEmpty()
        {
            CompleteInMethod(@"
                switch (this)
                {
                    default:
                        $
                        continue;
                }");

            AssertBody(
                new SwitchBlock
                {
                    Reference = VarRef("this"),
                    DefaultSection =
                    {
                        Fix.EmptyCompletion,
                        new ContinueStatement()
                    }
                });
        }

        [Test]
        public void CompletionInSwitchBlock_Nested()
        {
            CompleteInMethod(@"
                switch (this)
                {
                    default:
                        continue;
                        $
                }");

            AssertBody(
                new SwitchBlock
                {
                    Reference = VarRef("this"),
                    DefaultSection =
                    {
                        new ContinueStatement(),
                        Fix.EmptyCompletion
                    }
                });
        }

        [Test]
        public void CompletionInSwitchBlock_Nested2()
        {
            CompleteInMethod(@"
                switch (this)
                {
                    default:
                        int i;
                        continue;
                        $
                }");

            AssertBody(
                new SwitchBlock
                {
                    Reference = VarRef("this"),
                    DefaultSection =
                    {
                        VarDecl("i", Fix.Int),
                        new ContinueStatement(),
                        Fix.EmptyCompletion
                    }
                });
        }

        [Test]
        public void CompletionInSwitchBlock_Nested3()
        {
            CompleteInMethod(@"
                switch (this)
                {
                    default:
                        continue;
                        $
                        int i;
                }");

            AssertBody(
                new SwitchBlock
                {
                    Reference = VarRef("this"),
                    DefaultSection =
                    {
                        new ContinueStatement(),
                        Fix.EmptyCompletion,
                        VarDecl("i", Fix.Int)
                    }
                });
        }

        [Test]
        public void CompletionInSwitchBlock_After()
        {
            CompleteInMethod(@"
                switch ()
                {
                    default:
                        continue;
                }
                $
            ");

            AssertBody(
                new SwitchBlock
                {
                    DefaultSection = {new ContinueStatement()}
                },
                Fix.EmptyCompletion);
        }
    }
}
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
 *    - Andreas Bauer
 */

using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Blocks;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.LoopHeader;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.SSTPrintingVisitorTestSuite
{
    internal class BlockPrinterTest : SSTPrintingVisitorTestBase
    {
        [Test]
        public void ForEachLoop()
        {
            var sst = new ForEachLoop
            {
                Declaration = SSTUtil.Declare("e", TypeName.Get("T,P")),
                LoopedReference = SSTUtil.VariableReference("elements"),
                Body =
                {
                    new ContinueStatement()
                }
            };

            AssertPrint(
                sst,
                "foreach (T e in elements)",
                "{",
                "    continue;",
                "}");
        }

        [Test]
        public void SwitchBlock()
        {
            var sst = new SwitchBlock
            {
                Reference = SSTUtil.VariableReference("a"),
                Sections =
                {
                    new CaseBlock
                    {
                        Label = new ConstantValueExpression {Value = "1"},
                        Body = {new BreakStatement(), new BreakStatement()}
                    },
                    new CaseBlock {Label = new ConstantValueExpression {Value = "2"}, Body = {new BreakStatement()}}
                },
                DefaultSection = {new BreakStatement()}
            };

            AssertPrint(
                sst,
                "switch (a)",
                "{",
                "    case 1:",
                "        break;",
                "        break;",
                "    case 2:",
                "        break;",
                "    default:",
                "        break;",
                "}");
        }

        [Test]
        public void SwitchBlock_NoDefaultBlock()
        {
            var sst = new SwitchBlock
            {
                Reference = SSTUtil.VariableReference("a"),
                Sections =
                {
                    new CaseBlock
                    {
                        Label = new ConstantValueExpression {Value = "1"},
                        Body = {new BreakStatement(), new BreakStatement()}
                    },
                    new CaseBlock {Label = new ConstantValueExpression {Value = "2"}, Body = {new BreakStatement()}}
                }
            };

            AssertPrint(
                sst,
                "switch (a)",
                "{",
                "    case 1:",
                "        break;",
                "        break;",
                "    case 2:",
                "        break;",
                "}");
        }

        [Test]
        public void TryBlock()
        {
            var sst = new TryBlock
            {
                Body = {new ThrowStatement {Exception = TypeName.Get("ExceptionType,P")}},
                CatchBlocks =
                {
                    new CatchBlock
                    {
                        Exception = SSTUtil.Declare("e", TypeName.Get("ExceptionType,P")),
                        Body = {new BreakStatement()}
                    }
                },
                Finally = {new ContinueStatement()}
            };

            AssertPrint(
                sst,
                "try",
                "{",
                "    throw new ExceptionType();",
                "}",
                "catch (ExceptionType e)",
                // TODO: general catch blocks
                "{",
                "    break;",
                "}",
                "finally",
                "{",
                "    continue;",
                "}");
        }

        [Test]
        public void TryBlock_NoFinallyBlock()
        {
            var sst = new TryBlock
            {
                Body = {new ThrowStatement {Exception = TypeName.Get("ExceptionType,P")}},
                CatchBlocks =
                {
                    new CatchBlock
                    {
                        Exception = SSTUtil.Declare("e", TypeName.Get("ExceptionType,P")),
                        Body = {new BreakStatement()}
                    }
                }
            };

            AssertPrint(
                sst,
                "try",
                "{",
                "    throw new ExceptionType();",
                "}",
                "catch (ExceptionType e)",
                "{",
                "    break;",
                "}");
        }

        [Test]
        public void TryBlock_GeneralCatchBlock()
        {
            var sst = new TryBlock
            {
                Body = {new ThrowStatement {Exception = TypeName.Get("ExceptionType,P")}},
                CatchBlocks =
                {
                    new CatchBlock
                    {
                        Exception = new VariableDeclaration(), // empty/missing!
                        Body = {new BreakStatement()}
                    }
                }
            };

            AssertPrint(
                sst,
                "try",
                "{",
                "    throw new ExceptionType();",
                "}",
                "catch",
                "{",
                "    break;",
                "}");
        }

        [Test]
        public void UncheckedBlock()
        {
            var sst = new UncheckedBlock
            {
                Body = {new BreakStatement()}
            };

            AssertPrint(
                sst,
                "unchecked",
                "{",
                "    break;",
                "}");
        }

        [Test]
        public void UnsafeBlock()
        {
            var sst = new UnsafeBlock();

            AssertPrint(sst, "unsafe { /* content ignored */ }");
        }

        [Test]
        public void UsingBlock()
        {
            var sst = new UsingBlock
            {
                Reference = SSTUtil.VariableReference("variable"),
                Body = {new BreakStatement()}
            };

            AssertPrint(
                sst,
                "using (variable)",
                "{",
                "    break;",
                "}");
        }

        [Test]
        public void IfElseBlock()
        {
            var sst = new IfElseBlock
            {
                Condition = new ConstantValueExpression {Value = "true"},
                Then = {new ContinueStatement()},
                Else = {new BreakStatement()},
            };

            AssertPrint(
                sst,
                "if (true)",
                "{",
                "    continue;",
                "}",
                "else",
                "{",
                "    break;",
                "}");
        }

        [Test]
        public void IfElseBlock_NoElseBlock()
        {
            var sst = new IfElseBlock
            {
                Condition = new ConstantValueExpression {Value = "true"},
                Then = {new ContinueStatement()},
            };

            AssertPrint(
                sst,
                "if (true)",
                "{",
                "    continue;",
                "}");
        }

        [Test]
        public void LockBlock()
        {
            var sst = new LockBlock
            {
                Reference = SSTUtil.VariableReference("variable"),
                Body = {new ContinueStatement()}
            };

            AssertPrint(
                sst,
                "lock (variable)",
                "{",
                "    continue;",
                "}");
        }

        [Test]
        public void WhileLoop()
        {
            var sst = new WhileLoop
            {
                Condition = new LoopHeaderBlockExpression
                {
                    Body = // TODO: could be empty
                    {
                        new ReturnStatement {Expression = new ConstantValueExpression {Value = "true"}}
                    }
                },
                Body =
                {
                    new ContinueStatement(),
                    new BreakStatement(),
                }
            };

            AssertPrint(
                sst,
                "while (",
                "    {",
                "        return true;",
                "    }",
                ")",
                "{",
                "    continue;",
                "    break;",
                "}");
        }

        [Test]
        public void DoLoop()
        {
            var sst = new DoLoop
            {
                Condition = new LoopHeaderBlockExpression
                {
                    Body = // TODO: could be empty
                    {
                        new ReturnStatement {Expression = new ConstantValueExpression {Value = "true"}}
                    }
                },
                Body =
                {
                    new ContinueStatement(),
                    new BreakStatement(),
                }
            };

            AssertPrint(
                sst,
                "do",
                "{",
                "    continue;",
                "    break;",
                "}",
                "while (",
                "    {",
                "        return true;",
                "    }",
                ")");
        }

        [Test]
        public void ForLoop()
        {
            var sst = new ForLoop
            {
                Init =
                {
                    SSTUtil.Declare("i", TypeName.Get("T,P")),
                    SSTUtil.AssignmentToLocal("i", new ConstantValueExpression {Value = "0"})
                },
                Body = {new ContinueStatement(), new BreakStatement()},
                Condition =
                    new LoopHeaderBlockExpression
                    {
                        Body = {new ReturnStatement {Expression = new ConstantValueExpression {Value = "true"}}}
                    },
                Step = {}
            };

            AssertPrint(
                sst,
                "for (",
                "    {",
                "        T i;",
                "        i = 0;",
                "    };",
                "    {",
                "        return true;",
                "    }; { }",
                ")",
                "{",
                "    continue;",
                "    break;",
                "}");
        }
    }
}
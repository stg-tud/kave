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
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Blocks
{
    internal class UncheckedBlockAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void Block_CompletionBefore()
        {
            CompleteInMethod(@"$ unchecked { return; }");

            AssertBody(
                Fix.EmptyCompletion,
                new UncheckedBlock {Body = {Fix.Return}});
        }

        [Test]
        public void Block_CompletionAfter()
        {
            CompleteInMethod(@"unchecked { return; } $");

            AssertBody(
                new UncheckedBlock {Body = {Fix.Return}},
                Fix.EmptyCompletion);
        }

        [Test]
        public void Block_CompletionWithin()
        {
            CompleteInMethod(@"unchecked { $ }");

            AssertBody(
                new UncheckedBlock {Body = {Fix.EmptyCompletion}});
        }

        [Test]
        public void Expression_ConstantValue()
        {
            CompleteInMethod(@"int i = unchecked(1 + 2); $");

            AssertBody(
                VarDecl("i", Fix.Int),
                VarDecl("$0", Fix.Int),
                new UncheckedBlock
                {
                    Body =
                    {
                        Assign(
                            "$0",
                            new BinaryExpression
                            {
                                LeftOperand = Const("1"),
                                Operator = BinaryOperator.Plus,
                                RightOperand = Const("2")
                            })
                    }
                },
                Assign("i", RefExpr("$0")),
                Fix.EmptyCompletion);
        }

        [Test]
        public void Expression_VariableReference()
        {
            CompleteInMethod(@"
                int j = 1;
                int i = unchecked(j + 2);
                $
            ");

            AssertBody(
                VarDecl("j", Fix.Int),
                Assign("j", Const("1")),
                VarDecl("i", Fix.Int),
                VarDecl("$0", Fix.Int),
                new UncheckedBlock
                {
                    Body =
                    {
                        Assign(
                            "$0",
                            new BinaryExpression
                            {
                                LeftOperand = RefExpr("j"),
                                Operator = BinaryOperator.Plus,
                                RightOperand = Const("2")
                            })
                    }
                },
                Assign("i", RefExpr("$0")),
                Fix.EmptyCompletion);
        }

        [Test]
        public void Expression_MethodCall()
        {
            CompleteInMethod(@"
               var i = unchecked(GetHashCode() + 1);
               $
            ");

            AssertBody(
                "M",
                VarDecl("i", Fix.Int),
                VarDecl("$1", Fix.Int),
                new UncheckedBlock
                {
                    Body =
                    {
                        VarDecl("$0", Fix.Int),
                        Assign("$0", Invoke("this", Fix.Object_GetHashCode)),
                        Assign(
                            "$1",
                            new BinaryExpression
                            {
                                LeftOperand = RefExpr("$0"),
                                Operator = BinaryOperator.Plus,
                                RightOperand = Const("1")
                            })
                    }
                },
                Assign("i", RefExpr("$1")),
                Fix.EmptyCompletion);
        }
    }
}
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
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Expressions.
    ComposedExpressionAnalysisTestSuite
{
    internal class ArithmeticExpressionTest : BaseSSTAnalysisTest
    {
        [Test]
        public void AddingTwoInts()
        {
            CompleteInMethod(@"
                var i = 1 + 2;
                $");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign(
                    "i",
                    new BinaryExpression
                    {
                        LeftOperand = Const("1"),
                        Operator = BinaryOperator.Plus,
                        RightOperand = Const("2")
                    }),
                Fix.EmptyCompletion);
        }

        [Test]
        public void AddingThreeInts()
        {
            CompleteInMethod(@"
                var i = 1 + 2 + 3;
                $");

            AssertBody(
                VarDecl("i", Fix.Int),
                VarDecl("$0", Fix.Int),
                Assign(
                    "$0",
                    new BinaryExpression
                    {
                        LeftOperand = Const("1"),
                        Operator = BinaryOperator.Plus,
                        RightOperand = Const("2")
                    }),
                Assign(
                    "i",
                    new BinaryExpression
                    {
                        LeftOperand = RefExpr("$0"),
                        Operator = BinaryOperator.Plus,
                        RightOperand = new ConstantValueExpression()
                    }),
                Fix.EmptyCompletion);
        }

        [Test]
        public void AddingVariables()
        {
            CompleteInMethod(@"
                var i = 1;
                var j = i + 2;
                $");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", Const("1")),
                VarDecl("j", Fix.Int),
                Assign(
                    "j",
                    new BinaryExpression
                    {
                        LeftOperand = RefExpr("i"),
                        Operator = BinaryOperator.Plus,
                        RightOperand = Const("2")
                    }),
                Fix.EmptyCompletion);
        }

        [Test]
        public void SubtractingTwoInts()
        {
            CompleteInMethod(@"
                var i = 1 - 2;
                $");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign(
                    "i",
                    new BinaryExpression
                    {
                        LeftOperand = Const("1"),
                        Operator = BinaryOperator.Minus,
                        RightOperand = Const("2")
                    }),
                Fix.EmptyCompletion);
        }

        [Test]
        public void MultiplyingTwoInts()
        {
            CompleteInMethod(@"
                var i = 1 * 2;
                $");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign(
                    "i",
                    new BinaryExpression
                    {
                        LeftOperand = Const("1"),
                        Operator = BinaryOperator.Multiply,
                        RightOperand = Const("2")
                    }),
                Fix.EmptyCompletion);
        }

        [Test]
        public void DividingTwoInts()
        {
            CompleteInMethod(@"
                var i = 1 / 2;
                $");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign(
                    "i",
                    new BinaryExpression
                    {
                        LeftOperand = Const("1"),
                        Operator = BinaryOperator.Divide,
                        RightOperand = Const("2")
                    }),
                Fix.EmptyCompletion);
        }

        [Test]
        public void ModuloTwoInts()
        {
            CompleteInMethod(@"
                var i = 1 % 2;
                $");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign(
                    "i",
                    new BinaryExpression
                    {
                        LeftOperand = Const("1"),
                        Operator = BinaryOperator.Modulo,
                        RightOperand = Const("2")
                    }),
                Fix.EmptyCompletion);
        }

        [Test]
        public void NestedArithmeticExpressions()
        {
            CompleteInMethod(@"
                var i = 1 + (2 * 3 - (4 / 5));
                $");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign(
                    "i",
                    new BinaryExpression
                    {
                        LeftOperand = Const("1"),
                        Operator = BinaryOperator.Plus,
                        RightOperand = new ConstantValueExpression()
                    }),
                Fix.EmptyCompletion);
        }
    }
}
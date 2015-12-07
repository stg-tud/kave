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

using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Expressions.
    ComposedExpressionAnalysisTestSuite
{
    internal class BitwiseExpressionTests : BaseSSTAnalysisTest
    {
        [Test]
        public void BitwiseAndOnTwoBools()
        {
            CompleteInMethod(@"
                var i = true & false;
                $");

            AssertBody(
                VarDecl("i", Fix.Bool),
                Assign("i", new ConstantValueExpression()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void BitwiseAndOnTwoInts()
        {
            CompleteInMethod(@"
                var i = 101 & 1;
                $");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", new ConstantValueExpression()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void BitwiseOrOnThreeInts()
        {
            CompleteInMethod(@"
                var i = 1 & 2 & 3;
                $");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", new ConstantValueExpression()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void BitwiseOrOnTwoBools()
        {
            CompleteInMethod(@"
                var i = true | false;
                $");

            AssertBody(
                VarDecl("i", Fix.Bool),
                Assign("i", new ConstantValueExpression()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void BitwiseOrOnTwoInts()
        {
            CompleteInMethod(@"
                var i = 101 | 1;
                $");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", new ConstantValueExpression()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void BitwiseXorOnTwoBools()
        {
            CompleteInMethod(@"
                var i = true ^ false;
                $");

            AssertBody(
                VarDecl("i", Fix.Bool),
                Assign("i", new ConstantValueExpression()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void BitwiseXorOnTwoInts()
        {
            CompleteInMethod(@"
                var i = 101 ^ 1;
                $");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", new ConstantValueExpression()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void BitwiseAndWithVariables()
        {
            CompleteInMethod(@"
                var i = false;
                var j = true & i;
                $");

            AssertBody(
                VarDecl("i", Fix.Bool),
                Assign("i", Const("false")),
                VarDecl("j", Fix.Bool),
                Assign("j", ComposedExpr("i")),
                Fix.EmptyCompletion);
        }

        [Test]
        public void BitwiseOrOnTwoValues()
        {
            CompleteInMethod(@"
                var i = true | false;
                $");

            AssertBody(
                VarDecl("i", Fix.Bool),
                Assign("i", new ConstantValueExpression()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void BitwiseLeftShift()
        {
            CompleteInMethod(@"
                var i = 100 << 1;
                $");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", new ConstantValueExpression()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void BitwiseRightShift()
        {
            CompleteInMethod(@"
                var i = 100 >> 1;
                $");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", new ConstantValueExpression()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void BitwiseComplement()
        {
            CompleteInMethod(@"
                var i = ~100;
                $");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", new ConstantValueExpression()),
                Fix.EmptyCompletion);
        }
    }
}
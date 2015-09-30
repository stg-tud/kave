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
    internal class BooleanExpressionTest : BaseSSTAnalysisTest
    {
        [Test]
        public void BooleanAndOnTwoValues()
        {
            CompleteInMethod(@"
                var i = true && false;
                $");

            AssertBody(
                VarDecl("i", Fix.Bool),
                VarAssign("i", ComposedExpr()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void BooleanAndOnThreeValues()
        {
            CompleteInMethod(@"
                var i = true && false && true;
                $");

            AssertBody(
                VarDecl("i", Fix.Bool),
                VarAssign("i", ComposedExpr()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void BooleanAndWithVariables()
        {
            CompleteInMethod(@"
                var i = false;
                var j = true && i;
                $");

            AssertBody(
                VarDecl("i", Fix.Bool),
                VarAssign("i", new ConstantValueExpression()),
                VarDecl("j", Fix.Bool),
                VarAssign("j", ComposedExpr("i")),
                Fix.EmptyCompletion);
        }

        [Test]
        public void BooleanOrOnTwoValues()
        {
            CompleteInMethod(@"
                var i = true || false;
                $");

            AssertBody(
                VarDecl("i", Fix.Bool),
                VarAssign("i", ComposedExpr()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void BooleanEqualityOnTwoValues()
        {
            CompleteInMethod(@"
                var i = 1 == 2;
                $");

            AssertBody(
                VarDecl("i", Fix.Bool),
                VarAssign("i", ComposedExpr()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void BooleanInequalityOnTwoValues()
        {
            CompleteInMethod(@"
                var i = 1 != 2;
                $");

            AssertBody(
                VarDecl("i", Fix.Bool),
                VarAssign("i", ComposedExpr()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void NegationExpression()
        {
            CompleteInMethod(@"
                var i = !false;
                $");

            AssertBody(
                VarDecl("i", Fix.Bool),
                VarAssign("i", ComposedExpr()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void Relational_Greater()
        {
            CompleteInMethod(@"
                var i = 2 > 1;
                $");

            AssertBody(
                VarDecl("i", Fix.Bool),
                VarAssign("i", ComposedExpr()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void Relational_GreaterOrEqual()
        {
            CompleteInMethod(@"
                var i = 2 >= 1;
                $");

            AssertBody(
                VarDecl("i", Fix.Bool),
                VarAssign("i", ComposedExpr()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void Relational_Less()
        {
            CompleteInMethod(@"
                var i = 2 < 1;
                $");

            AssertBody(
                VarDecl("i", Fix.Bool),
                VarAssign("i", ComposedExpr()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void Relational_LessOrEqual()
        {
            CompleteInMethod(@"
                var i = 2 <= 1;
                $");

            AssertBody(
                VarDecl("i", Fix.Bool),
                VarAssign("i", ComposedExpr()),
                Fix.EmptyCompletion);
        }
    }
}
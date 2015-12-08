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

using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Expressions
{
    internal class IfElseExpressionAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void ConstantTernaryExpression()
        {
            CompleteInMethod(@"
                var i = true ? 1 : 2;
                $");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign(
                    "i",
                    new IfElseExpression
                    {
                        Condition = Const("true"),
                        ThenExpression = Const("1"),
                        ElseExpression = Const("2")
                    }),
                Fix.EmptyCompletion);
        }

        [Test]
        public void TernaryExpressionWithVariables()
        {
            CompleteInMethod(@"
                var b = true;
                var i = b ? 1 : 2;
                $");

            AssertBody(
                VarDecl("b", Fix.Bool),
                Assign("b", Const("true")),
                VarDecl("i", Fix.Int),
                Assign(
                    "i",
                    new IfElseExpression
                    {
                        Condition = RefExpr("b"),
                        ThenExpression = Const("1"),
                        ElseExpression = Const("2")
                    }),
                Fix.EmptyCompletion);
        }

        // TODO: Figure out how to handle these

        [Test, Ignore]
        public void ConstantTernaryExpression_CompletionWithin()
        {
            CompleteInMethod(@"
                var i = 1 == $ ? 3 : 4;
                ");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign(
                    "i",
                    new IfElseExpression
                    {
                        Condition = new ConstantValueExpression(),
                        ThenExpression = new ConstantValueExpression(),
                        ElseExpression = new ConstantValueExpression()
                    }),
                Fix.EmptyCompletion);
        }

        [Test, Ignore]
        public void ConstantTernaryExpression_CompletionWithin2()
        {
            CompleteInMethod(@"
                var i = $ == 2 ? 3 : 4;
                ");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign(
                    "i",
                    new IfElseExpression
                    {
                        Condition = new ConstantValueExpression(),
                        ThenExpression = new ConstantValueExpression(),
                        ElseExpression = new ConstantValueExpression()
                    }),
                Fix.EmptyCompletion);
        }
    }
}
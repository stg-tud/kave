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
    internal class BasicTest : BaseSSTAnalysisTest
    {
        [Test]
        public void Casts()
        {
            CompleteInMethod(@"
                var i = (float)1;
                $");

            AssertBody(
                VarDecl("i", Fix.Float),
                VarAssign("i", ComposedExpr()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void CastingMethodResult()
        {
            CompleteInClass(@"
                public int GetInt() { return 1; }
                public void M() 
                {
                    var i = (float)GetInt();
                    $
                }");

            AssertBody(
                "M",
                VarDecl("i", Fix.Float),
                VarDecl("$0", Fix.Int),
                VarAssign("$0", Invoke("this", Fix.Method(Fix.Int, Type("C"), "GetInt"))),
                VarAssign("i", ComposedExpr("$0")),
                Fix.EmptyCompletion);
        }

        [Test]
        public void TopLevelParentheses()
        {
            CompleteInMethod(@"
                var i = (1 + 2);
                $");

            AssertBody(
                VarDecl("i", Fix.Int),
                VarAssign("i", ComposedExpr()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void TernaryExpression()
        {
            CompleteInMethod(@"
                var i = 1 == 2 ? 3 : 4;
                $");

            AssertBody(
                VarDecl("i", Fix.Int),
                VarAssign("i", ComposedExpr()),
                Fix.EmptyCompletion);
        }

        [Test, Ignore]
        public void Assignment()
        {
            CompleteInMethod(@"
                var i = 100;
                var x = i = 101;
                $");

            AssertBody(
                VarDecl("i", Fix.Bool),
                VarAssign("i", new ConstantValueExpression()),
                // TODO
                Fix.EmptyCompletion);
        }
    }
}
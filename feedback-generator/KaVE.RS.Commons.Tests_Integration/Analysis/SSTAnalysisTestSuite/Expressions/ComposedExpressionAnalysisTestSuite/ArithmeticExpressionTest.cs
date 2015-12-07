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
                Assign("i", new ConstantValueExpression()),
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
                Assign("i", new ConstantValueExpression()),
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
                Assign("j", ComposedExpr("i")),
                Fix.EmptyCompletion);
        }

        [Test]
        public void AddingConstantToMethodResult()
        {
            CompleteInClass(@"
                public int GetInt() { return 1; }
                public void M() 
                {
                    var i = 1 + GetInt();
                    $
                }");

            AssertBody(
                "M",
                VarDecl("i", Fix.Int),
                VarDecl("$0", Fix.Int),
                Assign("$0", Invoke("this", Fix.Method(Fix.Int, Type("C"), "GetInt"))),
                Assign("i", ComposedExpr("$0")),
                Fix.EmptyCompletion);
        }

        [Test]
        public void AddingConstantToMethodResult_Chained()
        {
            CompleteInClass(@"
                public int GetInt() { return 1; }
                public C NewInstance() { return new C(); }
                public void M() 
                {
                    var i = 1 + NewInstance().GetInt();
                    $
                }");

            AssertBody(
                "M",
                VarDecl("i", Fix.Int),
                VarDecl("$0", Type("C")),
                Assign("$0", Invoke("this", Fix.Method(Type("C"), Type("C"), "NewInstance"))),
                VarDecl("$1", Fix.Int),
                Assign("$1", Invoke("$0", Fix.Method(Fix.Int, Type("C"), "GetInt"))),
                Assign("i", ComposedExpr("$1")),
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
                Assign("i", new ConstantValueExpression()),
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
                Assign("i", new ConstantValueExpression()),
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
                Assign("i", new ConstantValueExpression()),
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
                Assign("i", new ConstantValueExpression()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void Modulus()
        {
            CompleteInMethod(@"
                var i = 100 % 6;
                $");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", new ConstantValueExpression()),
                Fix.EmptyCompletion);
        }
    }
}
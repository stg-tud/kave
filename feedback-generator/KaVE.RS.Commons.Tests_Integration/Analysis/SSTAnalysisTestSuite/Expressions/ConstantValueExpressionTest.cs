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

using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Expressions
{
    internal class ConstantValueExpressionAnalysisTest : BaseSSTAnalysisTest
    {
        [TestCase(-2, null),
         TestCase(-1, "-1"),
         TestCase(0, "0"),
         TestCase(1, "1"),
         TestCase(2, "2"),
         TestCase(3, null)]
        public void IntValue(int before, string after)
        {
            CompleteInMethod(@"
                var i = " + before + @";
                $
            ");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", Const(after)),
                Fix.EmptyCompletion);
        }

        [Test]
        public void PlusOperator()
        {
            CompleteInMethod(@"
                var i = +1;
                $
            ");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", Const("1")),
                Fix.EmptyCompletion);
        }

        [TestCase("-1.23", null),
         TestCase("-1.0", "-1.0"),
         TestCase("0.0", "0.0"),
         TestCase("0.5", null),
         TestCase("1.0", "1.0"),
         TestCase("1.00001", null),
         TestCase("1.23", null)]
        public void DoubleValue(string before, string after)
        {
            CompleteInMethod(@"
                var d = " + before + @";
                $
            ");

            AssertBody(
                VarDecl("d", Fix.Double),
                Assign("d", Const(after)),
                Fix.EmptyCompletion);
        }

        [TestCase("true"),
         TestCase("false")]
        public void BooleanValue(string value)
        {
            CompleteInMethod(@"
                var b = " + value + @";
                $
            ");

            AssertBody(
                VarDecl("b", Fix.Bool),
                Assign("b", Const(value)),
                Fix.EmptyCompletion);
        }

        [TestCase("\"x\"", null),
         TestCase("\"1\"", null)]
        public void StringValue(string before, string after)
        {
            CompleteInMethod(@"
                var s = " + before + @";
                $
            ");

            AssertBody(
                VarDecl("s", Fix.String),
                Assign("s", Const(after)),
                Fix.EmptyCompletion);
        }

        [Test]
        public void DefaultValue()
        {
            CompleteInMethod(@"
                var i = default(int);
                $
            ");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", Const("default")),
                Fix.EmptyCompletion);
        }

        [Test]
        public void TypeofExpression()
        {
            CompleteInMethod(@"
                var t = typeof(int);
                $
            ");

            AssertBody(
                VarDecl("t", Fix.Type),
                Assign("t", Const("typeof")),
                Fix.EmptyCompletion);
        }

        [Test]
        public void SizeofExpression()
        {
            CompleteInMethod(@"
                var s = sizeof(int);
                $
            ");

            AssertBody(
                VarDecl("s", Fix.Int),
                Assign("s", Const("sizeof")),
                Fix.EmptyCompletion);
        }

        [Test]
        public void NullExpression()
        {
            CompleteInMethod(@"
                object o = null;
                $
            ");

            AssertBody(
                VarDecl("o", Fix.Object),
                Assign("o", Const("null")),
                Fix.EmptyCompletion);
        }
    }
}
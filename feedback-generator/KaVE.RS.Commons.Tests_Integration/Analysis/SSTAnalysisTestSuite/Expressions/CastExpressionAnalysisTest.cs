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
    class CastExpressionAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void CastingConstantValue_Alias()
        {
            CompleteInMethod(@"
                var i = (float)1;
                $");

            AssertBody(
                VarDecl("i", Fix.Float),
                VarDecl("$0", Fix.Int),
                VarAssign("$0", new ConstantValueExpression()),
                VarAssign("i", new CastExpression {TargetType = Fix.Float, VariableReference = VarRef("$0")}),
                SSTAnalysisFixture.EmptyCompletion);
        }

        [Test]
        public void CastingConstantValue_ClassType()
        {
            CompleteInMethod(@"
                var i = (Single)1;
                $");

            AssertBody(
                VarDecl("i", Fix.Float),
                VarDecl("$0", Fix.Int),
                VarAssign("$0", new ConstantValueExpression()),
                VarAssign("i", new CastExpression { TargetType = Fix.Float, VariableReference = VarRef("$0") }),
                SSTAnalysisFixture.EmptyCompletion);
        }

        [Test]
        public void CastingConstantValue_ClassType_FullQualified()
        {
            CompleteInMethod(@"
                var i = (System.Single)1;
                $");

            AssertBody(
                VarDecl("i", Fix.Float),
                VarDecl("$0", Fix.Int),
                VarAssign("$0", new ConstantValueExpression()),
                VarAssign("i", new CastExpression { TargetType = Fix.Float, VariableReference = VarRef("$0") }),
                SSTAnalysisFixture.EmptyCompletion);
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
                VarAssign("i", new CastExpression {TargetType = Fix.Float, VariableReference = VarRef("$0")}),
                Fix.EmptyCompletion);
        }
    }
}

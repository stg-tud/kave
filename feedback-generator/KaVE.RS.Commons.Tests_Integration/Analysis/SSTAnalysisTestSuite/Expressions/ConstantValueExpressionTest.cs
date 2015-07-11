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
using KaVE.Commons.Model.SSTs.Impl.Statements;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Expressions
{
    internal class ConstantValueExpressionAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void IntValue()
        {
            CompleteInMethod(@"
                var i = 3;
                $
            ");

            AssertBody(
                new VariableDeclaration
                {
                    Reference = BaseSSTAnalysisTest.VarRef("i"),
                    Type = Fix.Int
                },
                new Assignment
                {
                    Reference = BaseSSTAnalysisTest.VarRef("i"),
                    Expression = new ConstantValueExpression()
                },
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
                new VariableDeclaration
                {
                    Reference = BaseSSTAnalysisTest.VarRef("i"),
                    Type = Fix.Int
                },
                new Assignment
                {
                    Reference = BaseSSTAnalysisTest.VarRef("i"),
                    Expression = new ConstantValueExpression()
                },
                Fix.EmptyCompletion);
        }

        // TODO: CodeCompletion cases

        // TODO: more smoke tests
    }
}
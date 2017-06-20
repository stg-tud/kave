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
        public void TopLevelParentheses()
        {
            CompleteInMethod(@"
                var i = (1 + 2);
                $");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", new ConstantValueExpression()),
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
                Assign("i", new ConstantValueExpression()),
                // TODO
                Fix.EmptyCompletion);
        }
    }
}
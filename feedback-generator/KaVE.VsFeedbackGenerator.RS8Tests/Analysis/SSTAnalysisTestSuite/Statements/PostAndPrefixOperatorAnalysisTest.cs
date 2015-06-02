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
 * 
 * Contributors:
 *    - Sebastian Proksch
 */

using JetBrains.ReSharper.Psi.CSharp.Tree;
using KaVE.VsFeedbackGenerator.Analysis.CompletionTarget;
using NUnit.Framework;
using Fix = KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite.Statements
{
    internal class PostAndPrefixOperatorAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void PostfixPlusPlus_Before()
        {
            CompleteInMethod(@"
                $
                i++;
            ");

            AssertCompletionMarker<IPostfixOperatorExpression>(CompletionCase.EmptyCompletionBefore);

            AssertBody(
                Fix.EmptyCompletion,
                VarAssign("i", Fix.ComposedExpr("i")));
        }

        [Test]
        public void PostfixMinusMinus_After()
        {
            CompleteInMethod(@"
                i--;
                $
            ");

            AssertCompletionMarker<IPostfixOperatorExpression>(CompletionCase.EmptyCompletionAfter);

            AssertBody(
                VarAssign("i", Fix.ComposedExpr("i")),
                Fix.EmptyCompletion);
        }

        [Test]
        public void PrefixPlusPlus_Before()
        {
            CompleteInMethod(@"
                $
                ++i;
            ");

            AssertCompletionMarker<IPrefixOperatorExpression>(CompletionCase.EmptyCompletionBefore);

            AssertBody(
                Fix.EmptyCompletion,
                VarAssign("i", Fix.ComposedExpr("i")));
        }

        [Test]
        public void PrefixMinusMinus_After()
        {
            CompleteInMethod(@"
                --i;
                $
            ");

            AssertCompletionMarker<IPrefixOperatorExpression>(CompletionCase.EmptyCompletionAfter);

            AssertBody(
                VarAssign("i", Fix.ComposedExpr("i")),
                Fix.EmptyCompletion);
        }
    }
}
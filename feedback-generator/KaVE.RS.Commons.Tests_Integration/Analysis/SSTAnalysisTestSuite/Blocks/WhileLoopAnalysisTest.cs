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

using JetBrains.ReSharper.Psi.CSharp.Tree;
using KaVE.Commons.Model.SSTs.Impl.Blocks;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.RS.Commons.Analysis.CompletionTarget;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Blocks
{
    internal class WhileLoopAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void BasicWithCompletionAfter()
        {
            CompleteInMethod(@"
                while (true) {}
                $
            ");

            AssertCompletionMarker<IWhileStatement>(CompletionCase.EmptyCompletionAfter);

            AssertBody(
                new WhileLoop
                {
                    Condition = new ConstantValueExpression()
                },
                Fix.EmptyCompletion);
        }

        [Test]
        public void BasicWithCompletionBefore()
        {
            TestAnalysisTrigger.IsPrintingType = true;
            CompleteInMethod(@"
                $
                while (true) {}
            ");

            AssertCompletionMarker<IWhileStatement>(CompletionCase.EmptyCompletionBefore);

            AssertBody(
                Fix.EmptyCompletion,
                new WhileLoop
                {
                    Condition = new ConstantValueExpression()
                });
        }

        [Test]
        public void BasicWithCompletionInBody()
        {
            CompleteInMethod(@"
                while (true) {
                    $
                }
            ");

            AssertCompletionMarker<IWhileStatement>(CompletionCase.InBody);

            AssertBody(
                new WhileLoop
                {
                    Condition = new ConstantValueExpression(),
                    Body =
                    {
                        Fix.EmptyCompletion
                    }
                });
        }

        // TODO: completion in loop header

        [Test]
        public void WithStatementInBody()
        {
            CompleteInMethod(@"
                while (true) {
                    int i;
                }
                $
            ");

            AssertBody(
                new WhileLoop
                {
                    Condition = new ConstantValueExpression(),
                    Body =
                    {
                        new VariableDeclaration
                        {
                            Reference = BaseSSTAnalysisTest.VarRef("i"),
                            Type = Fix.Int
                        }
                    }
                },
                Fix.EmptyCompletion);
        }
    }
}
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
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.RS.Commons.Analysis.CompletionTarget;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Blocks
{
    internal class ForLoopAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void CompletionBefore()
        {
            CompleteInMethod(@"
                $
                for(;;) {}
            ");

            AssertCompletionMarker<IForStatement>(CompletionCase.EmptyCompletionBefore);

            AssertBody(
                Fix.EmptyCompletion,
                new ForLoop
                {
                    Condition = new UnknownExpression()
                });
        }

        [Test]
        public void CompletionAfter()
        {
            CompleteInMethod(@"
                for(;;) {}
                $
            ");

            AssertCompletionMarker<IForStatement>(CompletionCase.EmptyCompletionAfter);

            AssertBody(
                new ForLoop
                {
                    Condition = new UnknownExpression()
                },
                Fix.EmptyCompletion);
        }

        [Test]
        public void Init_VariableDeclaration()
        {
            CompleteInMethod(@"
                for(var i = 0;;) {}
                $
            ");

            AssertCompletionMarker<IForStatement>(CompletionCase.EmptyCompletionAfter);

            AssertBody(
                new ForLoop
                {
                    Init =
                    {
                        VarDecl("i", Fix.Int),
                        Assign("i", Const("0"))
                    }
                },
                Fix.EmptyCompletion);
        }

        [Test]
        public void Init_MultipleStatements()
        {
            CompleteInMethod(@"
                int i,j;
                for(i=0,j=1;;) {}
                $
            ");

            AssertCompletionMarker<IForStatement>(CompletionCase.EmptyCompletionAfter);

            AssertBody(
                VarDecl("i", Fix.Int),
                VarDecl("j", Fix.Int),
                new ForLoop
                {
                    Init =
                    {
                        Assign("i", Const("0")),
                        Assign("j", Const("1"))
                    }
                },
                Fix.EmptyCompletion);
        }

        [Test]
        public void Condition()
        {
            CompleteInMethod(@"
                for(; false;) {}
                $
            ");

            AssertCompletionMarker<IForStatement>(CompletionCase.EmptyCompletionAfter);

            AssertBody(
                new ForLoop
                {
                    Condition = Const("false")
                },
                Fix.EmptyCompletion);
        }

        [Test]
        public void Step_SingleStatement()
        {
            CompleteInMethod(@"
                var i = 0;
                for(;;i++) {}
                $
            ");

            AssertCompletionMarker<IForStatement>(CompletionCase.EmptyCompletionAfter);

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", Const("0")),
                new ForLoop
                {
                    Step =
                    {
                        Assign("i", new ComposedExpression {References = {VarRef("i")}})
                    }
                },
                Fix.EmptyCompletion);
        }

        [Test]
        public void Step_MultipleStatements()
        {
            CompleteInMethod(@"
                var i = 0;
                for(;;i++,i++) {}
                $
            ");

            AssertCompletionMarker<IForStatement>(CompletionCase.EmptyCompletionAfter);

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", Const("0")),
                new ForLoop
                {
                    Step =
                    {
                        Assign("i", new ComposedExpression {References = {VarRef("i")}}),
                        Assign("i", new ComposedExpression {References = {VarRef("i")}})
                    }
                },
                Fix.EmptyCompletion);
        }

        [Test]
        public void Body_EmptyWithCompletion()
        {
            CompleteInMethod(@"
                for(;;) {
                    $
                }
            ");

            AssertCompletionMarker<IForStatement>(CompletionCase.InBody);

            AssertBody(
                new ForLoop
                {
                    Body = {Fix.EmptyCompletion}
                });
        }

        [Test]
        public void Body_WithStatement()
        {
            CompleteInMethod(@"
                for(;;) {
                    int i;
                    $
                }
            ");

            AssertCompletionMarker<ILocalVariableDeclaration>(CompletionCase.EmptyCompletionAfter);

            AssertBody(
                new ForLoop
                {
                    Condition = new UnknownExpression(),
                    Body =
                    {
                        VarDecl("i", Fix.Int),
                        Fix.EmptyCompletion
                    }
                });
        }

        // TODO add tests for completions in init/condition/step
    }
}
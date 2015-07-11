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

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite
{
    // only covers basic cases of declarations, assignments, constant int values, and different completion points
    internal class BasicTest : BaseSSTAnalysisTest
    {
        [Test]
        public void LonelyVariableDeclaration()
        {
            CompleteInMethod(@"
                int i;
                $
            ");

            AssertNodeIsVariableDeclaration("i", LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.EmptyCompletionAfter);

            AssertBody(
                VarDecl("i", Fix.Int),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void AssignmentOfConstantValue()
        {
            CompleteInMethod(@"
                int i = 1;
                $
            ");

            AssertNodeIsVariableDeclaration("i", LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.EmptyCompletionAfter);

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", new ConstantValueExpression()),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void PlainCompletion()
        {
            CompleteInClass(@"
                public void A() {
                    $
                }
            ");

            AssertNodeIsMethodDeclaration("A", LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.InBody);

            AssertBody(
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void CompletionBeforeDeclaration()
        {
            CompleteInMethod(@"
                $
                int i;
            ");

            AssertNodeIsVariableDeclaration("i", LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.EmptyCompletionBefore);

            AssertBody(
                ExprStmt(new CompletionExpression()),
                VarDecl("i", Fix.Int));
        }

        [Test]
        public void CompletionAfterDeclaration()
        {
            CompleteInMethod(@"
                int i;
                $
            ");

            AssertNodeIsVariableDeclaration("i", LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.EmptyCompletionAfter);

            AssertBody(
                VarDecl("i", Fix.Int),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void CompletionInBetweenDeclarations()
        {
            CompleteInMethod(@"
                int i;
                $
                int j;
            ");

            AssertNodeIsVariableDeclaration("i", LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.EmptyCompletionAfter);

            AssertBody(
                VarDecl("i", Fix.Int),
                ExprStmt(new CompletionExpression()),
                VarDecl("j", Fix.Int));
        }

        [Test]
        public void CompletionBeforeExpressionStatement()
        {
            CompleteInMethod(@"
                $
                int i = 1;
            ");

            AssertNodeIsVariableDeclaration("i", LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.EmptyCompletionBefore);

            AssertBody(
                ExprStmt(new CompletionExpression()),
                VarDecl("i", Fix.Int),
                Assign("i", new ConstantValueExpression()));
        }

        [Test]
        public void CompletionAfterExpressionStatement()
        {
            CompleteInMethod(@"
                int i = 1;
                $
            ");

            AssertNodeIsVariableDeclaration("i", LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.EmptyCompletionAfter);

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", new ConstantValueExpression()),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void CompletionInDeclarationInitializer()
        {
            CompleteInMethod(@"
                int i = $
            ");

            AssertNodeIsVariableDeclaration("i", LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.Undefined);

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", new CompletionExpression()));
        }

        [Test]
        public void CompletionInDeclarationInitializerWithoutSpace()
        {
            TestAnalysisTrigger.IsPrintingType = true;

            CompleteInMethod(@"
                int i=$
            ");

            AssertNodeIsVariableDeclaration("i", LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.Undefined);

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", new CompletionExpression()));
        }

        [Test]
        public void CompletionInDeclarationInitializerWithReference()
        {
            CompleteInMethod(@"
                int i = t$
            ");

            AssertNodeIsReference("t", LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.Undefined);

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign(
                    "i",
                    new CompletionExpression
                    {
                        Token = "t"
                    }));
        }

        [Test]
        public void CompletionInAssignment()
        {
            CompleteInMethod(@"
                int i;
                i = $
            ");

            AssertNodeIsAssignment("i", LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.EmptyCompletionAfter);

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", new CompletionExpression()));
        }

        [Test]
        public void CompletionInIfBody()
        {
            CompleteInMethod(@"
                if(true)
                {
                    $
                }
            ");

            AssertNodeIsIf(LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.InBody);

            AssertBody(
                new IfElseBlock
                {
                    Condition = new ConstantValueExpression(),
                    Then =
                    {
                        ExprStmt(new CompletionExpression())
                    }
                });
        }

        [Test]
        public void MethodCall()
        {
            CompleteInMethod(@"
                this.GetHashCode();
                $
            ");

            AssertCompletionMarker<IInvocationExpression>(CompletionCase.EmptyCompletionAfter);

            AssertBody(
                InvokeStmt("this", Fix.Object_GetHashCode),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void TriggeredOutsideOfMethods()
        {
            CompleteInClass(@"
                public static void M() {}
                $
            ");

            // TODO think about this simplification, perhaps it is better to create an artificial "no match" ITreeNode
            Assert.IsNull(LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.Undefined);
        }
    }
}